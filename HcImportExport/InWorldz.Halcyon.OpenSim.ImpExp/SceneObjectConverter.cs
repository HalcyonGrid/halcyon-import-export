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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenMetaverse;

namespace InWorldz.Halcyon.OpenSim.ImpExp
{
    /// <summary>
    /// Converts an opensim SceneObjectGroup to a Halcyon SceneObjectGroup
    /// </summary>
    public class SceneObjectConverter : System.IDisposable
    {
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
        public SceneObjectConverter(string openSimPath, Guid? creatorOverride = null, Guid? ownerOverride = null)
        {
            m_creatorOverride = creatorOverride.HasValue ? creatorOverride.Value : Guid.Empty;
            m_ownerOverride = ownerOverride.HasValue ? ownerOverride.Value : Guid.Empty;

            m_context = AppDomainContext.Create();

            IAssemblyTarget tgt = m_context.LoadAssembly(LoadMethod.LoadFile, Path.Combine(openSimPath, "OpenSim.Region.Framework.dll"));
            var assemblies = m_context.Domain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (asm.FullName.Contains("OpenSim.Region.Framework"))
                {
                    Type t = asm.GetType("OpenSim.Region.Framework.Scenes.Serialization.SceneObjectSerializer");

                    m_fromXml2Method = t.GetMethod("FromXml2Format", new Type[] { typeof(string) });
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

            return sshot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="osRootPart"></param>
        /// <returns></returns>
        private SceneObjectPartSnapshot ConvertOpenSimPartToPartSnapshot(dynamic osRootPart)
        {
            SceneObjectPartSnapshot snap = new SceneObjectPartSnapshot
            {
                AngularVelocity = new Vector3(osRootPart.AngularVelocity.X, osRootPart.AngularVelocity.Y, osRootPart.AngularVelocity.Z),
                AngularVelocityTarget = new Vector3(osRootPart.AngularVelocity.X, osRootPart.AngularVelocity.Y, osRootPart.AngularVelocity.Z),
                BaseMask = osRootPart.BaseMask,
                Category = osRootPart.Category,
                ClickAction = osRootPart.ClickAction,
                CollisionSound = osRootPart.CollisionSound.Guid,
                CollisionSoundVolume = osRootPart.CollisionSoundVolume,
                CreationDate = osRootPart.CreationDate,
                CreatorId = m_creatorOverride == Guid.Empty ? osRootPart.CreatorID.Guid : m_creatorOverride,
                Description = osRootPart.Description,
                EveryoneMask = osRootPart.EveryoneMask,
                Flags = osRootPart.Flags,
                FromItemId = Guid.Empty,
                GroupId = osRootPart.GroupID.Guid,
                GroupMask = osRootPart.GroupMask,
                GroupPosition = osRootPart.GroupPosition,
                HoverText = osRootPart.Text,
                Id = osRootPart.UUID.Guid,
                Inventory = ExtractSOPInventorySnapshot(osRootPart),
                OwnerId = m_ownerOverride == Guid.Empty ? osRootPart.OwnerID.Guid : m_ownerOverride,

            };

            return snap;
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
                Flags = osTaskItem.Flags
            };

            return tii;
        }

        public void Dispose()
        {
            m_context.Dispose();
        }
    }
}
