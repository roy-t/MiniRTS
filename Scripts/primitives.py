import bpy
import math
import mathutils
import random


def applyBoolean(operation, name, a, b):
    """For operation use either UNION, DIFFERENCE, or INTERSECT"""
    modifier = a.modifiers.new(type="BOOLEAN", name=name)
    modifier.object = b
    modifier.operation = operation

    bpy.context.scene.objects.active = a
    bpy.ops.object.modifier_apply(modifier=name, apply_as="DATA")

    bpy.data.objects.remove(b)


def createTetrahdron():
    Vertices = \
        [
            mathutils.Vector((0, -1 / math.sqrt(3), 0)),
            mathutils.Vector((0.5, 1 / (2 * math.sqrt(3)), 0)),
            mathutils.Vector((-0.5, 1 / (2 * math.sqrt(3)), 0)),
            mathutils.Vector((0, 0, math.sqrt(2 / 3))),
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


def createBody(prefix, builder):
    minHeight = 3
    maxHeight = 4
    height = randomFloat(minHeight, maxHeight)

    minBotRadius = 2.25
    maxBotRadius = 3
    botRadius = randomFloat(minBotRadius, maxBotRadius)

    minTopRadius = botRadius * 0.75
    maxTopRadius = botRadius * 1.1
    topRadius = randomFloat(minTopRadius, maxTopRadius)

    minVertices = 4
    maxVertices = 12
    vertices = random.randint(minVertices, maxVertices)

    builder.primitive_cone_add(
        location=(0, 0, 0),
        radius1=botRadius,
        radius2=topRadius,
        vertices=vertices,
        depth=height
    )
    
    body = bpy.context.object
    body.name = "{0}body".format(prefix)

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


def createTurret(prefix, seed):
    random.seed(seed)

    builder = bpy.ops.mesh
    createBody(prefix, builder)
    # maxHeight = 10.0
    # minHeight = 6.0
    # minRadius = 1.0
    # maxRadius = 4.0

    # height = randomFloat(minHeight, maxHeight)
    # radius = randomFloat(minRadius, maxRadius)
    # radiusBase = randomFloat(radius, radius + 2.0)

    # bpy.ops.mesh.primitive_cylinder_add(
    #     radius=radiusBase,
    #     location=(0, 0, 0)
    # )

    # base = bpy.context.object
    # base.name = "Base"

    # bpy.ops.mesh.primitive_cylinder_add(
    #     radius=radius,
    #     depth=height,
    #     location=(0, 0, 0)
    # )

    # torso = bpy.context.object
    # torso.name = "Torso"

    # applyBoolean("DIFFERENCE", "diff_base_torso", torso, base)
