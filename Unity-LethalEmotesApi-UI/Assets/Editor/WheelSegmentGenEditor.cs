using System;
using System.Collections.Generic;
using LethalEmotesApi.Ui;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Editor
{
    [CustomEditor(typeof(WheelSegmentGen))]
    public class WheelSegmentGenEditor : UnityEditor.Editor
    {
        [SerializeField]
        private string meshName = "";
        
        [SerializeField]
        private bool optimizeMesh;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDefaultInspector();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            
            inspector.Add(DrawSerializableFieldsInspector());
            
            var meshNameField = new TextField("Mesh name");
            meshNameField.RegisterValueChangedCallback(OnMeshNameFieldChanged);

            var optimizeMeshToggle = new Toggle("Optimize Mesh");
            optimizeMeshToggle.RegisterValueChangedCallback(OnOptimizeMeshToggleChanged);
            
            var genSegmentMeshButton = new Button(OnGenSegmentMeshButton);
            genSegmentMeshButton.Add(new Label("Gen Mesh"));

            inspector.Add(meshNameField);
            inspector.Add(optimizeMeshToggle);
            inspector.Add(genSegmentMeshButton);

            return inspector;
        }

        private VisualElement DrawSerializableFieldsInspector()
        {
            var box = new Box();
            
            var wheelSegmentGen = target as WheelSegmentGen;
            if (!wheelSegmentGen)
                return box;

            var segRow = BoxRow();
            var segmentField = new IntegerField("Segments")
            {
                bindingPath = "segments",
                style =
                {
                    minWidth = 160
                }
            };
            segRow.Add(segmentField);
            var segmentSlider = new SliderInt(0, 20)
            {
                bindingPath = "segments",
                style =
                {
                    flexGrow = 1
                }
            };
            segRow.Add(segmentSlider);
            box.Add(segRow);

            var offsetRow = BoxRow();
            var offsetField = new FloatField("Offset")
            {
                bindingPath = "offset",
                style =
                {
                    minWidth = 160
                }
            };
            offsetRow.Add(offsetField);
            var offsetSlider = new Slider(0, 90)
            {
                bindingPath = "offset",
                style =
                {
                    flexGrow = 1
                }
            };
            offsetRow.Add(offsetSlider);
            box.Add(offsetRow);
            
            var minRadRow = BoxRow();
            var minRadField = new FloatField("Min Radius")
            {
                bindingPath = "minRadius",
                style =
                {
                    minWidth = 160
                }
            };
            minRadRow.Add(minRadField);
            var minRadSlider = new Slider(0, 699)
            {
                bindingPath = "minRadius",
                style =
                {
                    flexGrow = 1
                }
            };
            minRadRow.Add(minRadSlider);
            box.Add(minRadRow);
            
            var maxRadRow = BoxRow();
            var maxRadField = new FloatField("Max Radius")
            {
                bindingPath = "maxRadius",
                style =
                {
                    minWidth = 160
                }
            };
            maxRadRow.Add(maxRadField);
            var maxRadSlider = new Slider(0, 700)
            {
                bindingPath = "maxRadius",
                style =
                {
                    flexGrow = 1
                }
            };
            maxRadRow.Add(maxRadSlider);
            box.Add(maxRadRow);

            return box;
        }

        private VisualElement BoxRow()
        {
            var row = new Box
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                }
            };

            return row;
        }

        private void OnMeshNameFieldChanged(ChangeEvent<string> evt)
        {
            meshName = evt.newValue;
        }
        
        private void OnOptimizeMeshToggleChanged(ChangeEvent<bool> evt)
        {
            optimizeMesh = evt.newValue;
        }

        private void OnGenSegmentMeshButton()
        {
            var wheelSegmentGen = target as WheelSegmentGen;
            if (!wheelSegmentGen)
                return;
            
            float degPer = (float)(Math.PI * 2 / wheelSegmentGen.segments);
            
            float currentRad = degPer + Mathf.Deg2Rad * wheelSegmentGen.offset;
            
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            int vertIndex = 0;
            float step = (float)Math.PI / 180;
            
            double max = (degPer + step / 2) - step;

            for (float i = 0; i < degPer + step / 2; i += step)
            {
                if (i + step >= degPer + step / 2)
                    break;

                float rad = currentRad + i;

                float minX = (float)(Math.Cos(rad) * wheelSegmentGen.minRadius);
                float minY = (float)(Math.Sin(rad) * wheelSegmentGen.minRadius);
                Vector3 curMin = new Vector3(minX, minY, 0);

                float maxX = (float)(Math.Cos(rad) * wheelSegmentGen.maxRadius);
                float maxY = (float)(Math.Sin(rad) * wheelSegmentGen.maxRadius);
                Vector3 curMax = new Vector3(maxX, maxY, 0);

                rad = currentRad + i + step;

                minX = (float)(Math.Cos(rad) * wheelSegmentGen.minRadius);
                minY = (float)(Math.Sin(rad) * wheelSegmentGen.minRadius);
                Vector3 nextMin = new Vector3(minX, minY, 0);

                maxX = (float)(Math.Cos(rad) * wheelSegmentGen.maxRadius);
                maxY = (float)(Math.Sin(rad) * wheelSegmentGen.maxRadius);
                Vector3 nextMax = new Vector3(maxX, maxY, 0);

                vertices.Add(curMin);
                vertices.Add(curMax);
                vertices.Add(nextMin);
                vertices.Add(nextMax);

                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex);

                triangles.Add(vertIndex + 3);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                
                double curU = i / max;
                double nextU = (i + step) / max;

                uvs.Add(new Vector2((float)curU, 0)); // 0, 0
                uvs.Add(new Vector2((float)curU, 1)); // 0, 1
                uvs.Add(new Vector2((float)nextU, 0)); // 1, 0
                uvs.Add(new Vector2((float)nextU, 1)); // 1, 1

                normals.Add(Vector3.back);
                normals.Add(Vector3.back);
                normals.Add(Vector3.back);
                normals.Add(Vector3.back);

                vertIndex += 4;
            }
            
            var segmentMesh = new Mesh
            {
                name = "EmoteWheel Segment Mesh",
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray(),
                normals = normals.ToArray()
            };
            
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", meshName, "asset");
            if (string.IsNullOrEmpty(path))
                return;

            path = FileUtil.GetProjectRelativePath(path);

            if (optimizeMesh)
                MeshUtility.Optimize(segmentMesh);
            
            AssetDatabase.CreateAsset(segmentMesh, path);
            AssetDatabase.SaveAssets();
        }
    }
}