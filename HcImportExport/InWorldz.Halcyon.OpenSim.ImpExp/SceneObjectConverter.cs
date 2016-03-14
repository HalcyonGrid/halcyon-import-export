/*
 * Copyright (c) 2015, InWorldz Halcyon Developers
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice, this
 *     list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright notice,
 *     this list of conditions and the following disclaimer in the documentation
 *     and/or other materials provided with the distribution.
 * 
 *   * Neither the name of halcyon nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using AppDomainToolkit;
using InWorldz.Region.Data.Thoosa.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenMetaverse;
using OpenSim.Framework;
using System.Collections.Generic;
using OpenMetaverse.StructuredData;

namespace InWorldz.Halcyon.OpenSim.ImpExp
{
    /// <summary>
    /// Converts an opensim SceneObjectGroup to a Halcyon SceneObjectGroup
    /// </summary>
    public class SceneObjectConverter : System.IDisposable
    {
        private IAssetResolver m_assetResolver;
        private AppDomainContext<AssemblyTargetLoader, PathBasedAssemblyResolver> m_context;

        private MethodInfo m_fromXml2Method;

        private Guid m_creatorOverride;
        private Guid m_ownerOverride;
        

        /// <summary>
        /// Constructs a new sceneobject converter
        /// </summary>
        /// <param name="openSimPath">The path to a vanilla opensim bin directory</param>
        /// <param name="creatorOverride">Override for the embedded creator ID</param>
        /// <param name="ownerOverride">Override for the embedded owner ID</param>
        public SceneObjectConverter(string openSimPath, IAssetResolver ar, Guid? creatorOverride = null, Guid? ownerOverride = null)
        {
            m_assetResolver = ar;
            m_creatorOverride = creatorOverride ?? Guid.Empty;
            m_ownerOverride = ownerOverride ?? Guid.Empty;

            m_context = AppDomainContext.Create();

            m_context.LoadAssembly(LoadMethod.LoadFile, Path.Combine(openSimPath, "OpenSim.Region.Framework.dll"));
            var assemblies = m_context.Domain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (asm.FullName.Contains("OpenSim.Region.Framework"))
                {
                    Type t = asm.GetType("OpenSim.Region.Framework.Scenes.Serialization.SceneObjectSerializer");

                    m_fromXml2Method = t.GetMethod("FromXml2Format", new[] { typeof(string) });
                    if (m_fromXml2Method == null)
                    {
                        // never throw generic Exception - replace this with some other exception type
                        throw new Exception("FromXml2Format(string):  No such method exists.");
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes an OpenSim Xml2 serialized SceneObjectGroup into a SceneObjectGroupSnapshot
        /// </summary>
        /// <param name="xml">The XML blob representing the serialized object</param>
        /// <returns></returns>
        public SceneObjectGroupSnapshot SOGSnapshotFromOpenSimXml2(string xml)
        {
            dynamic osSog = m_fromXml2Method.Invoke(null, new object[] { xml });
            dynamic osRootPart = osSog.RootPart;

            SceneObjectPartSnapshot rootPartSnap = ConvertOpenSimPartToPartSnapshot(osRootPart);
            
            SceneObjectGroupSnapshot sshot = new SceneObjectGroupSnapshot();
            sshot.TaintedAttachment = false;
            sshot.TempAttachment = false;

            sshot.RootPart = rootPartSnap;

            return sshot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="osPart"></param>
        /// <returns></returns>
        private SceneObjectPartSnapshot ConvertOpenSimPartToPartSnapshot(dynamic osPart)
        {
            //we need server weight and streaming cost

            SceneObjectPartSnapshot snap = new SceneObjectPartSnapshot
            {
                AngularVelocity = new Vector3(osPart.AngularVelocity.X, osPart.AngularVelocity.Y, osPart.AngularVelocity.Z),
                AngularVelocityTarget = new Vector3(osPart.AngularVelocity.X, osPart.AngularVelocity.Y, osPart.AngularVelocity.Z),
                BaseMask = osPart.BaseMask,
                Category = osPart.Category,
                ClickAction = osPart.ClickAction,
                CollisionSound = osPart.CollisionSound.Guid,
                CollisionSoundVolume = osPart.CollisionSoundVolume,
                CreationDate = osPart.CreationDate,
                CreatorId = m_creatorOverride == Guid.Empty ? osPart.CreatorID.Guid : m_creatorOverride,
                Description = osPart.Description,
                EveryoneMask = osPart.EveryoneMask,
                Flags = osPart.Flags,
                FromItemId = Guid.Empty,
                GroupId = osPart.GroupID.Guid,
                GroupMask = osPart.GroupMask,
                GroupPosition = osPart.GroupPosition,
                HoverText = osPart.Text,
                Id = osPart.UUID.Guid,
                Inventory = ExtractSOPInventorySnapshot(osPart),
                KeyframeAnimation = ExtractSOPKFASnapshot(osPart),
                LastOwnerId = osPart.LastOwnerID.Guid,
                LinkNumber = osPart.LinkNum,
                LocalId = osPart.LocalId,
                Material = (Material)osPart.Material,
                MediaUrl = osPart.MediaUrl,
                Name = osPart.Name,
                NextOwnerMask = osPart.NextOwnerMask,
                ObjectFlags = (PrimFlags)osPart.ObjectFlags,
                ObjectSaleType = osPart.ObjectSaleType,
                OffsetPosition = osPart.OffsetPosition,
                OwnerMask = osPart.OwnerMask,
                OwnershipCost = osPart.OwnershipCost,
                ParentId = osPart.ParentID,
                ParticleSystem = osPart.ParticleSystem,
                PassTouches = osPart.PassTouches,
                PayPrice = osPart.PayPrice,
                RegionHandle = osPart.RegionHandle,
                RotationOffset = new Quaternion(osPart.RotationOffset.X, osPart.RotationOffset.Y, osPart.RotationOffset.Z, osPart.RotationOffset.W),
                SalePrice = osPart.SalePrice,
                Scale = osPart.Scale,
                ScriptAccessPin = osPart.ScriptAccessPin,
                ServerWeight = 1,
                StreamingCost = 1,
                Shape = ExtractSOPBaseShape(osPart),

                OwnerId = m_ownerOverride == Guid.Empty ? osPart.OwnerID.Guid : m_ownerOverride,
                
            };

            return snap;
        }

        private PrimShapeSnapshot ExtractSOPBaseShape(dynamic osPart)
        {
            Tuple<RenderMaterials, Dictionary<Guid, Guid>> matsWithRemap = ExtractRenderMaterials(osPart);

            return new PrimShapeSnapshot
            {
                ExtraParams = osPart.Shape.ExtraParams,
                FlexiDrag = osPart.Shape.FlexiDrag,
                FlexiEntry = osPart.Shape.FlexiEntry,
                FlexiForceX = osPart.Shape.FlexiForceX,
                FlexiForceY = osPart.Shape.FlexiForceY,
                FlexiForceZ = osPart.Shape.FlexiForceZ,
                FlexiGravity = osPart.Shape.FlexiGravity,
                FlexiSoftness = osPart.Shape.FlexiSoftness,
                FlexiTension = osPart.Shape.FlexiTension,
                FlexiWind = osPart.Shape.FlexiWind,
                HighLODBytes = osPart.Shape.HighLODBytes,
                HollowShape = (global::OpenSim.Framework.HollowShape)(int)osPart.Shape.HollowShape,
                LightColor = new float[] { osPart.Shape.LightColorR, osPart.Shape.LightColorG,
                    osPart.Shape.LightColorB, osPart.Shape.LightColorA },
                LightCutoff = osPart.Shape.LightCutoff,
                LightEntry = osPart.Shape.LightEntry,
                LightIntensity = osPart.Shape.LightIntensity,
                LightRadius = osPart.Shape.LightRadius,
                LowestLODBytes = osPart.Shape.LowestLODBytes,
                LowLODBytes = osPart.Shape.LowLODBytes,
                MediaList = ExtractMediaEntrySnapshot(osPart),
                MidLODBytes = osPart.Shape.MidLODBytes,
                PathBegin = osPart.Shape.PathBegin,
                PCode = osPart.Shape.PCode,
                PathCurve = osPart.Shape.PathCurve,
                PathEnd = osPart.Shape.PathEnd,
                PathRadiusOffset = osPart.Shape.PathRadiusOffset,
                PathRevolutions = osPart.Shape.PathRevolutions,
                PathScaleX = osPart.Shape.PathScaleX,
                PathScaleY = osPart.Shape.PathScaleY,
                PathShearX = osPart.Shape.PathShearX,
                PathShearY = osPart.Shape.PathShearY,
                PathSkew = osPart.Shape.PathSkew,
                PathTaperX = osPart.Shape.PathTaperX,
                PathTaperY = osPart.Shape.PathTaperY,
                PathTwist = osPart.Shape.PathTwist,
                PathTwistBegin = osPart.Shape.PathTwistBegin,
                PreferredPhysicsShape = PhysicsShapeType.Prim, //TODO: Is this supported on OS? 
                ProfileBegin = osPart.Shape.ProfileBegin,
                ProfileCurve = osPart.Shape.ProfileCurve,
                ProfileEnd = osPart.Shape.ProfileEnd,
                ProfileHollow = osPart.Shape.ProfileHollow,
                ProfileShape = osPart.Shape.ProfileShape,
                ProjectionAmbiance = osPart.Shape.ProjectionAmbiance,
                ProjectionEntry = osPart.Shape.ProjectionEntry,
                ProjectionFOV = osPart.Shape.ProjectionFOV,
                ProjectionFocus = osPart.Shape.ProjectionFocus,
                ProjectionTextureId = osPart.Shape.ProjectionTextureUUID.Guid,
                RenderMaterials = matsWithRemap.Item1,
                
            };
        }

        private Tuple<RenderMaterials, Dictionary<Guid, Guid>>  ExtractRenderMaterials(dynamic osPart)
        {
            var te = new Primitive.TextureEntry(osPart.Shape.TextureEntry, 0, osPart.Shape.TextureEntry.Length);
            var materialTextureIds = new List<Guid>();

            if (te.DefaultTexture != null)
            {
                materialTextureIds.Add(te.DefaultTexture.MaterialID.Guid);
            }

            foreach (Primitive.TextureEntryFace face in te.FaceTextures)
            {
                if (face != null)
                {
                    materialTextureIds.Add(face.MaterialID.Guid);
                }
            }

            var mats = new RenderMaterials();
            var matRemap = new Dictionary<Guid, Guid>();

            foreach (Guid matId in materialTextureIds)
            {
                byte[] osdBlob = m_assetResolver.ResolveAsset(matId);
                if (osdBlob != null)
                {
                    OSD osd = OSDParser.DeserializeLLSDXml(osdBlob);
                    matRemap.Add(matId, mats.AddMaterial(RenderMaterial.FromOSD(osd)).Guid);
                }
            }

            return new Tuple<RenderMaterials, Dictionary<Guid, Guid>>(mats, matRemap);
        }

        private MediaEntrySnapshot[] ExtractMediaEntrySnapshot(dynamic osPart)
        {
            dynamic mediaList = osPart.Shape.Media;
            if (mediaList == null) return null;

            MediaEntrySnapshot[] snaps = new MediaEntrySnapshot[mediaList.Count];
            for (int i = 0; i < mediaList.Count; i++)
            {
                dynamic currMedia = mediaList[i];
                snaps[i] = new MediaEntrySnapshot
                {
                    AutoLoop = currMedia.AutoLoop,
                    AutoPlay = currMedia.AutoPlay,
                    AutoScale = currMedia.AutoScale,
                    AutoZoom = currMedia.AutoZoom,
                    ControlPermissions = (MediaPermission)(int)currMedia.ControlPermissions,
                    Controls = (MediaControls)(int)currMedia.Controls,
                    CurrentURL = currMedia.CurrentURL,
                    EnableAlterntiveImage = currMedia.EnableAlternativeImage,
                    EnableWhiteList = currMedia.EnableWhiteList,
                    Height = currMedia.Height,
                    HomeURL = currMedia.HomeURL,
                    InteractOnFirstClick = currMedia.InteractOnFirstClick,
                    InteractPermissions = currMedia.InteractPermissions,
                    WhiteList = currMedia.WhiteList,
                    Width = currMedia.Width
                };
            }

            return snaps;
        }

        private KeyframeAnimationSnapshot ExtractSOPKFASnapshot(dynamic osPart)
        {
            //there are several private fields inside the OS KFM class
            //that make it impossible to extract through public members
            return null;
        }

        private TaskInventorySnapshot ExtractSOPInventorySnapshot(dynamic osPart)
        {
            dynamic itemList = osPart.TaskInventory;

            TaskInventorySnapshot invSnap = new TaskInventorySnapshot();
            invSnap.Items = new TaskInventoryItemSnapshot[itemList.Count];
            invSnap.Serial = osPart.InventorySerial;

            int i = 0;
            foreach (dynamic item in itemList)
            {
                invSnap.Items[i++] = ConvertOpenSimTaskInventoryItemToSnapshot(item.Value);
            }

            return invSnap;
        }

        private TaskInventoryItemSnapshot ConvertOpenSimTaskInventoryItemToSnapshot(dynamic osTaskItem)
        {
            TaskInventoryItemSnapshot tii = new TaskInventoryItemSnapshot
            {
                AssetId = osTaskItem.AssetID.Guid,
                BasePermissions = osTaskItem.BasePermissions,
                CreationDate = osTaskItem.CreationDate,
                CreatorId = m_creatorOverride == Guid.Empty ? osTaskItem.osTaskItem.CreatorID : m_creatorOverride,
                CurrentPermissions = osTaskItem.CurrentPermissions,
                Description = osTaskItem.Description,
                EveryonePermissions = osTaskItem.EveryonePermissions,
                Flags = osTaskItem.Flags,
                GroupId = osTaskItem.GroupID,
                GroupPermissions = osTaskItem.GroupPermissions,
                InvType = osTaskItem.InvType,
                ItemId = osTaskItem.ItemID,
                LastOwnerId = osTaskItem.LastOwnerID,
                Name = osTaskItem.Name,
                NextOwnerPermissions = osTaskItem.NextPermissions,
                OldItemId = osTaskItem.OldItemID.Guid,
                OwnerId = osTaskItem.OwnerID.Guid,
                ParentId = osTaskItem.ParentID.Guid,
                ParentPartId = osTaskItem.ParentPartID.Guid,
                PermsGranter = osTaskItem.PermsGranter.Guid,
                PermsMask = osTaskItem.PermsMask,
                Type = osTaskItem.Type
            };

            return tii;
        }

        public void Dispose()
        {
            m_context.Dispose();
        }
    }
}
