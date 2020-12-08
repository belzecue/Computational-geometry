using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habrador_Computational_Geometry
{
    //A collection of classes that implements the Half-Edge Data Structure
    //From https://www.openmesh.org/media/Documentations/OpenMesh-6.3-Documentation/a00010.html

    //3D space
    public class HalfEdgeData3
    {
        public HashSet<HalfEdgeVertex3> vertices;

        public HashSet<HalfEdgeFace3> faces;

        public HashSet<HalfEdge3> edges;



        public HalfEdgeData3()
        {
            this.vertices = new HashSet<HalfEdgeVertex3>();

            this.faces = new HashSet<HalfEdgeFace3>();

            this.edges = new HashSet<HalfEdge3>();
        }



        //Get a list with unique edges
        //Currently we have two half-edges for each edge, making it time consuming to go through them 
        public List<HalfEdge3> GetUniqueEdges()
        {
            List<HalfEdge3> uniqueEdges = new List<HalfEdge3>();

            foreach (HalfEdge3 e in edges)
            {
                MyVector3 p1 = e.v.position;
                MyVector3 p2 = e.prevEdge.v.position;

                bool isInList = false;

                for (int j = 0; j < uniqueEdges.Count; j++)
                {
                    HalfEdge3 testEdge = uniqueEdges[j];

                    MyVector3 p1_test = testEdge.v.position;
                    MyVector3 p2_test = testEdge.prevEdge.v.position;

                    if ((p1.Equals(p1_test) && p2.Equals(p2_test)) || (p2.Equals(p1_test) && p1.Equals(p2_test)))
                    {
                        isInList = true;

                        break;
                    }
                }

                if (!isInList)
                {
                    uniqueEdges.Add(e);
                }
            }

            return uniqueEdges;
        }



        //Convert to Unity mesh (if we know we have stored triangles in the data structure)
        public Mesh ConvertToUnityMesh(string name)
        {
            MyMesh myMesh = new MyMesh();
        
            //Loop through each triangle
            foreach (HalfEdgeFace3 f in faces)
            {
                //These should have been stored clock-wise
                HalfEdgeVertex3 v1 = f.edge.v;
                HalfEdgeVertex3 v2 = f.edge.nextEdge.v;
                HalfEdgeVertex3 v3 = f.edge.nextEdge.nextEdge.v;
                //HalfEdgeVertex3 v3 = f.edge.prevEdge.v;

                MyMeshVertex my_v1 = new MyMeshVertex(v1.position, v1.normal);
                MyMeshVertex my_v2 = new MyMeshVertex(v2.position, v2.normal);
                MyMeshVertex my_v3 = new MyMeshVertex(v3.position, v3.normal);

                int index1 = myMesh.AddVertexAndReturnIndex(my_v1, shareVertices: true);
                int index2 = myMesh.AddVertexAndReturnIndex(my_v2, shareVertices: true);
                int index3 = myMesh.AddVertexAndReturnIndex(my_v3, shareVertices: true);

                myMesh.AddTrianglePositions(index1, index2, index3);
            }


            Mesh unityMesh = myMesh.ConvertToUnityMesh(name, generateNormals: false);

            return unityMesh;
        }
    }



    //A position
    public class HalfEdgeVertex3
    {
        //The position of the vertex
        public MyVector3 position;
        //In 3d space we also need a normal
        public MyVector3 normal;

        //Each vertex references an half-edge that starts at this point
        //Might seem strange because each halfEdge references a vertex the edge is going to?
        public HalfEdge3 edge;



        public HalfEdgeVertex3(MyVector3 position)
        {
            this.position = position;
        }

        public HalfEdgeVertex3(MyVector3 position, MyVector3 normal)
        {
            this.position = position;

            this.normal = normal;
        }
    }



    //This face could be a triangle or whatever we need
    public class HalfEdgeFace3
    {
        //Each face references one of the halfedges bounding it
        //If you need the vertices, you can use this edge
        public HalfEdge3 edge;



        public HalfEdgeFace3(HalfEdge3 edge)
        {
            this.edge = edge;
        }
    }



    //An edge going in a direction
    public class HalfEdge3
    {
        //The vertex it points to
        public HalfEdgeVertex3 v;

        //The face it belongs to
        public HalfEdgeFace3 face;

        //The next half-edge inside the face (ordered clockwise)
        //The document says counter-clockwise but clockwise is easier because that's how Unity is displaying triangles
        public HalfEdge3 nextEdge;

        //The opposite half-edge belonging to the neighbor (if there's a neighbor)
        public HalfEdge3 oppositeEdge;

        //(optionally) the previous halfedge in the face
        //If we assume the face is closed, then we could identify this edge by walking forward until we reach it
        public HalfEdge3 prevEdge;



        public HalfEdge3(HalfEdgeVertex3 v)
        {
            this.v = v;
        }
    }
}
