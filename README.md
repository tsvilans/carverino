# Carverino

![alt text](https://raw.githubusercontent.com/tsvilans/carverino/bunny.png)

"Carve is a fast, robust constructive solid geometry library. (fork from https://code.google.com/p/carve/)"

https://github.com/VTREEM/Carve

"CarveSharp is a .NET wrapper for the fast and robust constructive solid geometry (CSG) library Carve. Using CarveSharp, you could easily pass triangular meshes and perform boolean operations on them (such as union, intersect, etc.). CarveSharp is targeted for .NET v4 and above (due to the use of parallel for loops for increased performance). It can be easily integrated into Unity by rewriting all Parallel.For loops as regular C# for loops (note that the performance may significantly decrease)."

https://github.com/Maghoumi/CarveSharp

CarveRhino and CarveGH are an adaptation of the two wonderful pieces of software described above, allowing the usage of the Carve library in Rhino and Grasshopper, respectively. At the moment, just the basic operation of Carve is exposed, and outputs a triangulated mesh. Although Carve supports N-gons, Rhino doesn't, so these are instead triangulated. Hopefully this will change in the future with N-gon support in Rhino. There seems to be a lot of functionality in Carve that is not being exploited, so hopefully this can provide a good enough starting point to have good, solid mesh booleans in Rhino.

The libs are provided as-is, with no guarantee of support for now, as I use them internally and do not intend to develop this into a shiny, polished plug-in.

---

CarveLibWrapper.dll - The actual wrapper for the Carve library.

CarveSharp.dll - The dotNET assembly which exposes Carve, using only basic types.

CarveRC.dll - CarveRhinoCommon, which provides basic conversion from Rhino types (Mesh) to Carve types.

CarveGH.gha - Grasshopper assembly which adds the 'Carve' component to Mesh -> Util.

CarveRhino.rhp - Rhino plug-in which adds the 'Carve' command to Rhino.

---

This is currently structured in this way to keep it modular and allow people to use any particular part of the wrapper, with or without RhinoCommon or GH, etc.

This would not have been possible without the work of Mehran Maghoumi who created the original CarveSharp wrapper (https://github.com/Maghoumi). I have basically just removed dependencies to OpenTK and CodeFullToolkit, slightly re-organized the code, exposed some more functionality, and provided interfaces to Rhino and GH.

# Contact

tsvi@kadk.dk

http://tomsvilans.com
