import sys
import os
import os.path
import bpy
import bmesh
import datetime
from math import sqrt, radians, pi, cos, sin
from mathutils import Vector, Matrix
from random import random, seed, uniform, randint, randrange
from enum import IntEnum
from colorsys import hls_to_rgb

# Inspired by: https://github.com/a1studmuffin/SpaceshipGenerator


# Extrudes a face along its normal by length units.
# Returns the new face, and optionally fills out faces_list
# with all the additional side faces created from the extrusion.
def extrude_face(bm, face, length, faces_list=None):
    result = bmesh.ops.extrude_discrete_faces(bm, faces=[face])
    new_faces = result['faces']
    if(faces_list != None):
        faces_list += new_faces[:]
    
    new_face = new_faces[0]
    bmesh.ops.translate(bm, vec=new_face.normal * length, verts=new_face.verts)
    return new_face

# Similar to extrude_face, except corrigates the geometry to create "ribs".
# Returns the new face.
def ribbed_extrude_face(bm, face, length, rib_count=3, rib_scale=0.9):
    rib_length = length / float(rib_count)
    new_face = face
    for i in range(rib_count):
        new_face = extrude_face(bm, new_face, rib_length * 0.25)
        new_face = extrude_face(bm, new_face, 0.0)
        scale_face(bm, new_face, rib_scale, rib_scale, rib_scale)
        new_face = extrude_face(bm, new_face, rib_length * 0.5)
        new_face = extrude_face(bm, new_face, 0.0)
        scale_face(bm, new_face, 1 / rib_scale, 1 / rib_scale, 1 / rib_scale)
        new_face = extrude_face(bm, new_face, rib_length * 0.25)
    return new_face


# Scales a face in local face space
def scale_face(bm, face, x, y, z):
    face_space = get_face_matrix(face)
    face_space.invert()
    bmesh.ops.scale(bm, vec=Vector((x, y, z)), space=face_space,verts=face.verts)

    
# Returns a rough 4x4 transform matrix for a face (doesn't handle
# distortion/shear) with optional position override.
def get_face_matrix(face, pos=None):
    x_axis = (face.verts[1].co - face.verts[0].co).normalized()
    z_axis = -face.normal
    y_axis = z_axis.cross(x_axis)
    if not pos:
        pos = face.calc_center_bounds()

    # Construct a 4x4 matrix from axes + position:
    # http://i.stack.imgur.com/3TnQP.png
    mat = Matrix()
    mat[0][0] = x_axis.x
    mat[1][0] = x_axis.y
    mat[2][0] = x_axis.z
    mat[3][0] = 0
    mat[0][1] = y_axis.x
    mat[1][1] = y_axis.y
    mat[2][1] = y_axis.z
    mat[3][1] = 0
    mat[0][2] = z_axis.x
    mat[1][2] = z_axis.y
    mat[2][2] = z_axis.z
    mat[3][2] = 0
    mat[0][3] = pos.x
    mat[1][3] = pos.y
    mat[2][3] = pos.z
    mat[3][3] = 1
    return mat

# Generates a short base with differently scaled segments 
# and optionally ribbed surfaces
def generate_base(bm, top_face):
    segments = randrange(1, 3)
    segment_range = range(segments)
    for i in segment_range:                
        segment_length = uniform(0.2, 0.6)
        
        if random() > 0.25:  
            top_face = extrude_face(bm, top_face, segment_length)
            
            segment_scale = uniform(0.6, 1.4)
            scale_face(bm, top_face, segment_scale, segment_scale, 1)
        else:
            rib_scale = uniform(0.75, 0.95)
            rib_count = randint(2, 4)
            top_face = ribbed_extrude_face(bm, top_face, segment_length, rib_count, rib_scale)

    return top_face

def generate_bearings(bm, top_face):
    dimple = uniform(-0.15, 0.0)
    top_face = extrude_face(bm, top_face, dimple)
    bearing_scale = uniform(0.3, 0.8)
    scale_face(bm, top_face, bearing_scale, bearing_scale, 1)
    bearing_length = uniform(abs(dimple) + 0.1, abs(dimple) + 0.3)
    top_face = extrude_face(bm, top_face, bearing_length)
    
    return (top_face, bearing_scale)

def generate_top(bm, top_face, bearing_scale):
    inv = 1.0 / bearing_scale
    top_face = extrude_face(bm, top_face, 0)
    scale_face(bm, top_face, inv, inv, 1)
    top_face = extrude_face(bm, top_face, 1)
    return top_face

def generate():
    current_seed = randint(0, pow(2, 31))
    print("Seed {0}".format(current_seed))
    seed(current_seed)
    prefix = "TURRET_"
    
    # Remove previous iterations
    for b in bpy.data.objects:
        if b.name.startswith(prefix):
            print({"INFO"}, "Removing {0}".format(b.name))
            bpy.data.objects.remove(b)
            
    # Create a cone to start out with
    bm = bmesh.new()  
    bmesh.ops.create_cone(
        bm,
        cap_ends=True,
        segments=6,
        diameter1=1.0,
        diameter2=1.0,
        depth=1.0)
                
    # Random scale
    scale_vector = Vector((1, 1, uniform(0.25, 0.5)))
    bmesh.ops.scale(bm, vec=scale_vector, verts=bm.verts)
    
    # Find the top facing face
    for face in bm.faces[:]:
        if face.normal.z > 0.5:
            top_face = face
    
    top_face = generate_base(bm, top_face)
    (top_face, bearing_scale) = generate_bearings(bm, top_face)
    top_face = generate_top(bm, top_face, bearing_scale)
                    
    # Finish up, write the bmesh into a new mesh
    mesh = bpy.data.meshes.new('Mesh')
    bm.to_mesh(mesh)
    bm.free()
    
    # Add the mesh to the scene
    scene = bpy.context.scene
    obj = bpy.data.objects.new("{0}body".format(prefix), mesh)
    scene.objects.link(obj)
    
    # Select and make active
    scene.objects.active = obj
    obj.select = True
    
    # Recenter the object to its center of mass
    bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS')
    ob = bpy.context.object
    ob.location = (0, 0, 0)
    
    # Add a fairly broad bevel modifier to angularize shape
    bevel_modifier = ob.modifiers.new('Bevel', 'BEVEL')
    bevel_modifier.width = uniform(5, 20)
    bevel_modifier.offset_type = 'PERCENT'
    bevel_modifier.segments = 2
    bevel_modifier.profile = uniform(0.0, 0.5)
    bevel_modifier.limit_method ='NONE'
#    bpy.ops.object.modifier_apply(modifier='Bevel', apply_as="DATA")



generate()