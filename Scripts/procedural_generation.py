import bpy
import primitives


class ProceduralGenerationPanel(bpy.types.Panel):
    """Panel in create tab of the tool shelf of the 3D view"""
    bl_space_type = "VIEW_3D"
    bl_region_type = "TOOLS"
    bl_context = "objectmode"
    bl_category = "Create"
    bl_label = "Procedural Generation"

    def draw(self, context):
        TheCol = self.layout.column(align=True)
        TheCol.operator("mesh.make_tetrahedron", text="Add Tetrahedron")
    # end draw

# end ProceduralGenerationPanel


class MakeTetrahedron(bpy.types.Operator):
    bl_idname = "mesh.make_tetrahedron"
    bl_label = "Add Tetrahedron"
    bl_options = {"UNDO"}    

    def invoke(self, context, event):
        primitives.createTetrahdron(context)        
        return {"FINISHED"}
    # end invoke

# end MakeTetrahedron


def register():
    bpy.utils.register_class(MakeTetrahedron)
    bpy.utils.register_class(ProceduralGenerationPanel)


def unregister():
    bpy.utils.unregister_class(MakeTetrahedron)
    bpy.utils.unregister_class(ProceduralGenerationPanel)
