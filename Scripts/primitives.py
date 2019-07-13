import bpy
import bmesh
import math
import random
from mathutils import Vector, Matrix


def setActive(activate):
    bpy.context.scene.objects.active = activate
    activate.select = True


def applyBoolean(operation, name, a, b): 
    """For operation use either UNION, DIFFERENCE, or INTERSECT"""
    modifier = a.modifiers.new(type="BOOLEAN", name=name)
    modifier.object = b
    modifier.operation = operation

    setActive(a)
    bpy.ops.object.modifier_apply(modifier=name, apply_as="DATA")

    bpy.data.objects.remove(b)


def placeOnGroundPlane(model):
    setActive(model)
    bpy.ops.object.origin_set(type="GEOMETRY_ORIGIN", center="BOUNDS")
    model.location[2] = model.dimensions[2] / 2.0


def createTetrahdron():
    Vertices = \
        [
            Vector((0, -1 / math.sqrt(3), 0)),
            Vector((0.5, 1 / (2 * math.sqrt(3)), 0)),
            Vector((-0.5, 1 / (2 * math.sqrt(3)), 0)),
            Vector((0, 0, math.sqrt(2 / 3))),
        ]
    NewMesh = bpy.data.meshes.new("Tetrahedron")
    NewMesh.from_pydata(
        Vertices,
        [],
        [[0, 1, 2], [0, 1, 3], [1, 2, 3], [2, 0, 3]]
        )
    NewMesh.update()
    return bpy.data.objects.new("Tetrahedron", NewMesh)


def randomFloat(fMin, fMax):
    return random.random() * (fMax - fMin) + fMin


def randomRatio(value, minFactor, maxFactor):
    fMin = value * minFactor
    fMax = value * maxFactor
    return randomFloat(fMin, fMax)


def createBody(prefix, builder):
    minHeight = 5
    maxHeight = 7
    height = randomFloat(minHeight, maxHeight)

    minBotRadius = 2.25
    maxBotRadius = 3
    botRadius = randomFloat(minBotRadius, maxBotRadius)

    minTopRadius = botRadius * 0.75
    maxTopRadius = botRadius * 1.1
    topRadius = randomFloat(minTopRadius, maxTopRadius)

    minVertices = 4
    maxVertices = 12
    vertices = random.randrange(minVertices, maxVertices, 2)

    builder.primitive_cone_add(
        location=(0, 0, 0),
        radius1=botRadius,
        radius2=topRadius,
        vertices=vertices,
        depth=height
    )

    body = bpy.context.object
    body.name = "{0}body".format(prefix)

    rotation = math.pi / vertices
    body.rotation_euler[2] = rotation * random.randint(0, 1)

    minBaseRadius = min(topRadius, botRadius)
    minMountWidth = minBaseRadius * (0.4)
    maxMountWidth = minBaseRadius * (0.9)
    mountWidth = randomFloat(minMountWidth, maxMountWidth)

    minMountHeight = height * 0.2
    maxMountHeight = height * 0.8
    mountHeight = randomFloat(minMountHeight, maxMountHeight)

    offset = max(botRadius, topRadius)

    builder.primitive_cube_add(
        location=(-offset, 0, 0)
    )
    mount = bpy.context.object
    mount.name = "{0}mount".format(prefix)
    mount.scale[0] = max(botRadius, topRadius)
    mount.scale[1] = mountWidth
    mount.scale[2] = mountHeight * 0.5  # Z is up

    applyBoolean("DIFFERENCE", "diff_body_mount", body, mount)

    return body


def extrudeScale(body, faceIndex, extrusion, scale):
    setActive(body)
    bpy.ops.object.mode_set(mode='EDIT')      # Toggle edit mode
    bpy.ops.mesh.select_mode(type='FACE')     # Change to face selection
    bm = bmesh.from_edit_mesh(body.data)

    # Select, and retrieve the face we need
    bm.faces.ensure_lookup_table()
    for f in bm.faces:
        f.select = False

    face = bm.faces[faceIndex]
    face.select = True

    # Perform the extrusion
    bpy.ops.mesh.extrude_faces_move(
        MESH_OT_extrude_faces_indiv={"mirror": False},
        TRANSFORM_OT_shrink_fatten={
            "value": extrusion,
        }
    )

    # Rebuild face data
    bm.faces.ensure_lookup_table()
    for f in bm.faces:
        f.select = False

    face = bm.faces[faceIndex]

    # Perform the scaling
    c = face.calc_center_median()
    T = Matrix.Translation(-c)
    S = Matrix.Scale(scale, 4)
    bmesh.ops.transform(bm, matrix=S, verts=face.verts, space=T)


def createBlockBody(prefix, builder):
    height = 0.2

    minBotWidth = 1.0
    maxBotWidth = 1.5
    botWidth = randomFloat(minBotWidth, maxBotWidth)

    builder.primitive_cube_add(
        location=(0, 0, 0),
    )

    body = bpy.context.object
    body.name = "{0}body".format(prefix)
    body.scale[0] = botWidth
    body.scale[1] = botWidth
    body.scale[2] = height
    
    # Face 4 is the bottom face
    # Face 5 is the top face

    extrudeScale(body, 5, -3, 0.5)
    return body


def createNeck(prefix, builder, body):
    bodyRadius = min(body.dimensions[0], body.dimensions[1]) / 2.0
    neckRadius = randomRatio(bodyRadius, 0.5, 0.8)

    height = randomRatio(body.dimensions[2], 0.3, 0.6)

    vertices = random.randrange(4, 12, 2)

    builder.primitive_cone_add(
        location=(0, 0, -height / 2.0),
        depth=height,
        radius1=neckRadius,
        radius2=neckRadius,
        vertices=vertices
    )

    neck = bpy.context.object
    neck.name = "{0}neck".format(prefix)

    rotation = math.pi / vertices
    neck.rotation_euler[2] = rotation * random.randint(0, 1)

    return neck


def createTurret(prefix, seed): 
    random.seed(seed)
    builder = bpy.ops.mesh

    # We build the turret from top to bottom, always placing the part
    # that was just build with its base on the ground plane

    body = createBlockBody(prefix, builder)
    # placeOnGroundPlane(body)

    # neck = createNeck(prefix, builder, body)
    # applyBoolean("UNION", "union_body_neck", body, neck)

    # placeOnGroundPlane(body)
    
    bpy.ops.object.mode_set(mode='OBJECT')
