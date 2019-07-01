import bpy
import math
import mathutils
import random


def applyBoolean(operation, name, a, b):
    """UNION, DIFFERENCE, or INTERSECT"""
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


def createTurret(seed):
    random.seed(seed)

    maxHeight = 10.0
    minHeight = 6.0
    minRadius = 1.0
    maxRadius = 4.0

    height = randomFloat(minHeight, maxHeight)    
    radius = randomFloat(minRadius, maxRadius)
    radiusBase = randomFloat(radius, radius + 2.0)

    bpy.ops.mesh.primitive_cylinder_add(
        radius=radiusBase,
        location=(0, 0, 0)
    )

    base = bpy.data.objects['Cylinder']
    base.name = "Base"

    bpy.ops.mesh.primitive_cylinder_add(
        radius=radius,
        depth=height,
        location=(0, 0, 0)
    )

    torso = bpy.data.objects['Cylinder']
    torso.name = "Torso"

    applyBoolean("DIFFERENCE", "diff_base_torso", torso, base)
