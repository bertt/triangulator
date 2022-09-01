# Triangulator

.NET 6 library for triangulating 3D PolyhedralSurface WKB geometries

## NuGet

https://www.nuget.org/packages/bertt.triangulator/

## Sample code

```
var buildingWkb = File.ReadAllBytes(@"testdata/building.wkb");
var wkbTriangulated = Triangulator.Triangulate(buildingWkb);
var triangulatedGeometry = (PolyhedralSurface)Geometry.Deserialize<WkbSerializer>(wkbTriangulated);
Assert.IsTrue(triangulatedGeometry.Geometries.Count == 22);
```

## Remarks

- Input wkb must be of type PolyhedralSurface, otherwise an error will occur;
- Triangulated geometry is returned as WKB (also PolyhedralSurface);
- Geometries with holes are not supported (yet).

## Method 

3D Triangulation is performed in 2D mode, by projecting each input polygon
to yz, zx or xy plane. The plane to project to is determined by the normal vector of the 
input polygon.

Pseudo code for calculating plane to project to (nb: using absolute normals here): 

```
yz: when (normal(x) > normal(y)) and (normal(x) > normal(z))
zx: else when (normal(y) > normal(z))
xy: all other cases
```

For triangulation the fast Earcut method is 
used. After triangulation the resulting triangles are 'unflattened' to get the 
3D triangles. 

After that, each triangle is checked to have the same direction as the original polygon by calculating
the dot product between the normal of the polygon and the normal of the triangle. If the dot 
product is negative, then the two vectors point in opposite directions and the triangle will be 
inverted.

## Benchmark

todo

## Unit Testing

In the unit test there is an conversion method from triangles to glTF 2.0 using SharpGLTF (https://github.com/vpenades/SharpGLTF)
for visual inspections.

## Dependencies

wkx-sharp - https://github.com/cschwarz/wkx-sharp for handling geometries

## History

2020-09-01: release 1.1: to NET 6
2020-06-10: release 1.0.3
2020-06-09: release 1.0.2
2020-06-08: release 1.0.1
2020-06-07: release 1.0.0



