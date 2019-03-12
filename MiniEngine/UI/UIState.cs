using Microsoft.Xna.Framework;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace MiniEngine.UI
{
    public sealed class UIState
    {
        private const string UIStateFile = "ui_state.xml";

        public UIState()
        {
            this.SelectedRenderTargets = new List<RenderTargetDescription>(0);
            this.DebugDisplay = DebugDisplay.None;
            this.Columns = 4;
            this.ShowGui = true;

            this.CameraPosition = Vector3.Zero;
            this.CameraLookAt = Vector3.Forward;

            this.Data = new SerializationData();

            this.EntityState = new EntityState();
        }

        public EntityState EntityState { get; set; }

        public bool ShowDemo;        
        public bool ShowLightsWindow;
        public int Columns;

        public bool ShowGui { get; set; }        
        public DebugDisplay DebugDisplay { get; set; }        
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraLookAt { get; set; }        


        [XmlIgnore]
        public List<RenderTargetDescription> SelectedRenderTargets { get; }

        [XmlIgnore]
        public RenderTargetDescription SelectedRenderTarget { get; set; }             


        [EditorBrowsable(EditorBrowsableState.Never)]
        public SerializationData Data { get; set; }

        public void Serialize(Vector3 cameraPosition, Vector3 cameraLookAt)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            this.CameraPosition = cameraPosition;
            this.CameraLookAt = cameraLookAt;

            this.Data.SelectedEntityInt = this.EntityState.SelectedEntity;
            this.Data.SelectedRenderTargetName = this.SelectedRenderTarget?.Name;
            this.Data.SelectedRenderTargetNames = this.SelectedRenderTargets.Select(rt => rt.Name).ToList();

            var serializer = new XmlSerializer(typeof(UIState));
            using (var stream = File.CreateText(UIStateFile))
            {
                serializer.Serialize(stream, this);
            }

            Thread.CurrentThread.CurrentCulture = culture;
        }

        public static UIState Deserialize(GBuffer gBuffer)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                var serializer = new XmlSerializer(typeof(UIState));
                using (var stream = File.OpenRead(UIStateFile))
                {
                    var uiState = (UIState)serializer.Deserialize(stream);

                    uiState.EntityState.SelectedEntity = new Entity(uiState.Data.SelectedEntityInt);
                    uiState.SelectedRenderTarget = gBuffer.RenderTargets.FirstOrDefault(rt => rt.Name.Equals(uiState.Data.SelectedRenderTargetName));
                    foreach (var name in uiState.Data.SelectedRenderTargetNames)
                    {
                        uiState.SelectedRenderTargets.Add(gBuffer.RenderTargets.First(rt => rt.Name.Equals(name)));
                    }

                    return uiState;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: could not deserialize {UIStateFile}, {e.ToString()}");
                return new UIState();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }            
        }

        public class SerializationData
        {            
            public int SelectedEntityInt { get; set; }
            public List<string> SelectedRenderTargetNames { get; set; }
            public string SelectedRenderTargetName { get; set; }
        }
    }
}
