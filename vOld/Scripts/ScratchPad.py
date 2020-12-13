import bpy
import bmesh
from math import sqrt
from mathutils import Vector, Matrix
from random import random, seed, uniform, randint, randrange

NO_SYMMETRY_INDEX = 99
# Inspired by: https://github.com/a1studmuffin/SpaceshipGenerator


# Get all faces connected to the given face
def get_connected_faces(face):
    connected_faces = []
    for edge in face.edges:
        for connected_face in edge.link_faces:
            if connected_face != face:
                connected_faces.append(connected_face)

    return connected_faces


# Extrudes a face along its normal by length units, returns the new face
def extrude_face(bm, face, length):
    result = bmesh.ops.extrude_discrete_faces(bm, faces=[face])
    new_faces = result['faces']
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


# Returns the rough length and width of a quad face.
# Assumes a perfect rectangle, but close enough.
def get_face_width_and_height(face):
    if not face.is_valid or len(face.verts[:]) < 4:
        return -1, -1
    width = (face.verts[2].co - face.verts[1].co).length
    height = (face.verts[0].co - face.verts[1].co).length
    return width, height


# Returns the rough aspect ratio of a face. Always >= 1.
def get_aspect_ratio(face):
    if not face.is_valid:
        return 1.0
    face_aspect_ratio = max(0.01, face.edges[0].calc_length() / face.edges[1].calc_length())
    if face_aspect_ratio < 1.0:
        face_aspect_ratio = 1.0 / face_aspect_ratio
    return face_aspect_ratio


# Scales a face in local face space
def scale_face(bm, face, x, y, z):
    face_space = get_face_matrix(face)
    face_space.invert()
    bmesh.ops.scale(bm, vec=Vector((x, y, z)), space=face_space, verts=face.verts)


# Given a face, splits it up into a smaller uniform grid and extrudes each grid cell.
def add_grid_to_face(bm, face):
    if not face.is_valid:
        return
    result = bmesh.ops.subdivide_edges(bm,
                                    edges=face.edges[:],
                                    cuts=randint(2, 4),
                                    fractal=0.02,
                                    use_grid_fill=True,
                                    use_single_edge=False)
    grid_length = uniform(0.025, 0.15)
    scale = 0.8
    for face in result['geom']:
        if isinstance(face, bmesh.types.BMFace):
            face = extrude_face(bm, face, grid_length)
            scale_face(bm, face, scale, scale, scale)


# Given a face, adds some pointy intimidating antennas.
def add_surface_antenna_to_face(bm, face):
    if not face.is_valid or len(face.verts[:]) < 4:
        return
    horizontal_step = randint(4, 10)
    vertical_step = randint(4, 10)
    for h in range(horizontal_step):
        top = face.verts[0].co.lerp(
            face.verts[1].co, (h + 1) / float(horizontal_step + 1))
        bottom = face.verts[3].co.lerp(
            face.verts[2].co, (h + 1) / float(horizontal_step + 1))
        for v in range(vertical_step):
            if random() > 0.9:
                pos = top.lerp(bottom, (v + 1) / float(vertical_step + 1))
                face_size = sqrt(face.calc_area())
                depth = uniform(0.1, 1.5) * face_size
                depth_short = depth * uniform(0.02, 0.15)
                base_diameter = uniform(0.005, 0.05)

                # Spire
                num_segments = uniform(3, 6)
                result = bmesh.ops.create_cone(bm,
                                               cap_ends=False,
                                               cap_tris=False,
                                               segments=num_segments,
                                               diameter1=0,
                                               diameter2=base_diameter,
                                               depth=depth,
                                               matrix=get_face_matrix(face, pos + face.normal * depth * 0.5))

                # Base
                result = bmesh.ops.create_cone(bm,
                                               cap_ends=True,
                                               cap_tris=False,
                                               segments=num_segments,
                                               diameter1=base_diameter * uniform(1, 1.5),
                                               diameter2=base_diameter * uniform(1.5, 2),
                                               depth=depth_short,
                                               matrix=get_face_matrix(face, pos + face.normal * depth_short * 0.45))

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

        if random() > 0.45:
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

    top_face.material_index = NO_SYMMETRY_INDEX
    for f in get_connected_faces(top_face):
        f.material_index = NO_SYMMETRY_INDEX

    bearing_length = uniform(abs(dimple) + 0.1, abs(dimple) + 0.3)
    top_face = extrude_face(bm, top_face, bearing_length)

    # Make sure faces extruded from this face do not also get the NO_SYMMETRY_INDEX
    top_face.material_index = 0
    for f in get_connected_faces(top_face):
        f.material_index = NO_SYMMETRY_INDEX

    return (top_face, bearing_scale)


def generate_top(bm, top_face, bearing_scale):
    inv = 1.0 / bearing_scale
    top_face = extrude_face(bm, top_face, 0)
    scale_face(bm, top_face, inv, inv, 1)

    for f in get_connected_faces(top_face):
        f.material_index = NO_SYMMETRY_INDEX

    top_face = extrude_face(bm, top_face, uniform(0.5, 1.25))
    faces_list = get_connected_faces(top_face)
    return (top_face, faces_list)


def add_barrel(bm, face):
    face_width, face_height = get_face_width_and_height(face)
    nozzle_ratio = uniform(0.1, 0.25)
    nozzle_size = nozzle_ratio * min(face_width, face_height)
    nozzle_depth = uniform(0.5, 1.0)

    segments = randrange(1, 3)
    segment_range = range(segments)
    for i in segment_range:
        segment_length = uniform(0.1, 0.5)

        if random() > 0.45:
            face = extrude_face(bm, face, segment_length)
            segment_scale = nozzle_size * 2 * uniform(2.0, 3.5)
            scale_face(bm, face, segment_scale, segment_scale, 1)
        else:
            rib_scale = uniform(0.75, 0.95)
            rib_count = randint(2, 4)
            face = ribbed_extrude_face(bm, face, segment_length, rib_count, rib_scale)

    sphere_matrix = get_face_matrix(face,
                                    face.calc_center_bounds() + face.normal * nozzle_depth * 0.5)

    bmesh.ops.create_cone(bm,
                          cap_ends=False,
                          segments=12,
                          diameter1=nozzle_size,
                          diameter2=uniform(0.7, 1.3) * nozzle_size,
                          depth=nozzle_depth,
                          matrix=sphere_matrix)

    return face


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
    (top_face, side_faces) = generate_top(bm, top_face, bearing_scale)

    asymmetry_faces = []
    grid_faces = []
    antenna_faces = []

    num_asymmetry_segments_min = 1
    num_asymmetry_segments_max = 3
    for face in bm.faces[:]:
        # Skip specifically excluded faces
        if face.material_index == NO_SYMMETRY_INDEX:
            continue
        # Skip any long thin faces as it'll probably look stupid
        if get_aspect_ratio(face) > 3:
            continue
        # Skip small faces
        #if face.calc_area() < 1.0:
        #    face.select = True
        if random() > 0.9:
            asymmetry_faces.append(face)
        elif random() > 0.95:
            grid_faces.append(face)
        elif face.normal.z > 0.85:
            antenna_faces.append(face)

    # Add some large asymmetrical sections of the hull that stick out
    for face in asymmetry_faces:
        hull_piece_length = uniform(0.1, 0.4)
        for i in range(randrange(num_asymmetry_segments_min, num_asymmetry_segments_max)):
            face = extrude_face(bm, face, hull_piece_length)

            # Maybe apply some scaling
            if random() > 0.25:
                s = 1 / uniform(1.1, 1.5)
                scale_face(bm, face, s, s, s)

    # Find the heightest forward facing face
    best_face = None
    best_face_z = 0
    for face in bm.faces[:]:
        if face.normal.x > 0.99 and face.normal.z < 0.01 and face.normal.y < 0.01:
            face_z = face.calc_center_median()[2]
            if best_face is None or face_z > best_face_z:
                best_face = face
                best_face_z = face_z

    # Place the barrel
    add_barrel(bm, best_face)

    for face in grid_faces:
        add_grid_to_face(bm, face)

    for face in antenna_faces:
        add_surface_antenna_to_face(bm, face)

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
    obj.select = False

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
    bevel_modifier.limit_method = 'NONE'
#   bpy.ops.object.modifier_apply(modifier='Bevel', apply_as="DATA")

    solidify_modifier = ob.modifiers.new('Solidify', 'SOLIDIFY')
    solidify_modifier.thickness = 0.03



generate()
