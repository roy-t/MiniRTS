# How to add a 3D model

*For Blender 2.8:*

1. Create a 3D model as you would normally do. 
2. Double check the normals are facing outwards by going into edit mode, and selecting 'show normals' in the viewport overlays. (icon in top right of the viewport that shows to overlapping circles)
3. UV Unwrap the model
4. Create a texture file in `path/to/model/textures`
5. Add a material, in the node view set the Base Color to reference the texture file you've just created by clicking the little dot next to it
7. Verify the material works by pressing z (while in object mode in the viewport) and select LookDev
8. Export the file as FBX store it in the same folder as you've stored the model so all relative paths are preserved. Make sure to disable "Add end-bones" in the export armature settings (for performance)
    - Make sure to set the scale in the exporter from 1.00 to 0.01. (Blender works in meters and assumes FBX works with centimeters)
9. Copy the model to the content directory, copy the texture to `/path/to/model/in/content/directory/textures`.
10. Create the file `<modelname>.ini` and fill it with the material description (see other files for an example)
11. Add the model to MonoGame's content tool and set the processor to the multi-material model processor.
12. You can now use the model in game!


## Scale
Note that a model exported by blender with dimensions 1x1x1 will show up in the game as having dimensions 100x100x100, so apply a scale matrix of 0.01 over all dimensions to get the original proportions. 
I'm not sure why this happens, must be something in Blender's FBX exporter or MonoGame's model importer.

## Tips
- Rigging: https://continuebreak.com/articles/how-rig-vehicle-blender-28-ue4/
- UV unwrapping: go to UV view, go to edit mode, select all vertices, UV -> Unwrap, then select all islands in the UV editor and choose Pack Islands from the UV menu to get a good start.