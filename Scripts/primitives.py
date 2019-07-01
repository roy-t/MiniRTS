import bpy
import math
import mathutils
import random


def applyBoolean(operation, name, a, b):
    """For operation use either UNION, DIFFERENCE, or INTERSECT"""
    modifier = a.modifiers.new(type="BOOLEAN", name=name)
    modifier.object = b
    modifier.operation = operation
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
    minHeight = 0.5
    maxHeight = 2
    height = randomFloat(minHeight, maxHeight)

    minBotRadius = 0.5
    maxBotRadius = 3
    botRadius = randomFloat(minBotRadius, maxBotRadius)

    minTopRadius = botRadius * 0.5
    maxTopRadius = botRadius * 1.25
    topRadius = randomFloat(minTopRadius, maxTopRadius)

    minVertices = 3
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

    # bpy.ops.mesh_primitive_

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
