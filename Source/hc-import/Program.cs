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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InWorldz.Halcyon.OpenSim.ImpExp;

namespace InWorldz.Halcyon.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: doesn't compile at the moment...
            // TODO: Should be pulling the path from a CLI parameter...
            //SceneObjectConverter soc = new SceneObjectConverter("C:\\Projects\\InWorldz\\opensim\\bin");
            //soc.SOGSnapshotFromOpenSimXml2("<SceneObjectGroup><SceneObjectPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><AllowedDrop>false</AllowedDrop><CreatorID><UUID>47704d5f-910f-46ac-a685-7dcdf7bad9f3</UUID></CreatorID><FolderID><UUID>53e7448e-9a77-4451-98c7-b79e0b340e3d</UUID></FolderID><InventorySerial>0</InventorySerial><UUID><UUID>53e7448e-9a77-4451-98c7-b79e0b340e3d</UUID></UUID><LocalId>2574477159</LocalId><Name>Babys Breath</Name><Material>3</Material><PassTouches>false</PassTouches><PassCollisions>false</PassCollisions><RegionHandle>1099511628032000</RegionHandle><ScriptAccessPin>0</ScriptAccessPin><GroupPosition><X>149.3031</X><Y>128.8885</Y><Z>23.21969</Z></GroupPosition><OffsetPosition><X>0</X><Y>0</Y><Z>0</Z></OffsetPosition><RotationOffset><X>0</X><Y>0</Y><Z>0</Z><W>1</W></RotationOffset><Velocity><X>0</X><Y>0</Y><Z>0</Z></Velocity><AngularVelocity><X>0</X><Y>0</Y><Z>0</Z></AngularVelocity><Acceleration><X>0</X><Y>0</Y><Z>0</Z></Acceleration><Description /><Color><R>0</R><G>0</G><B>0</B><A>255</A></Color><Text /><SitName /><TouchName /><LinkNum>0</LinkNum><ClickAction>0</ClickAction><Shape><ProfileCurve>0</ProfileCurve><TextureEntry>SIoAY0krTaGlNpr+UIuR6QAAAAAAAAAAAEAAAACAQQAAQAAAQAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA</TextureEntry><ExtraParams>ATAAEQAAAM4u+b5GuEYTmG5TBM7AJK8D</ExtraParams><PathBegin>0</PathBegin><PathCurve>32</PathCurve><PathEnd>0</PathEnd><PathRadiusOffset>0</PathRadiusOffset><PathRevolutions>0</PathRevolutions><PathScaleX>100</PathScaleX><PathScaleY>150</PathScaleY><PathShearX>0</PathShearX><PathShearY>0</PathShearY><PathSkew>0</PathSkew><PathTaperX>0</PathTaperX><PathTaperY>0</PathTaperY><PathTwist>0</PathTwist><PathTwistBegin>0</PathTwistBegin><PCode>9</PCode><ProfileBegin>0</ProfileBegin><ProfileEnd>0</ProfileEnd><ProfileHollow>0</ProfileHollow><State>0</State><ProfileShape>Circle</ProfileShape><HollowShape>Same</HollowShape><SculptTexture><UUID>ce2ef9be-46b8-4613-986e-5304cec024af</UUID></SculptTexture><SculptType>3</SculptType><SculptData></SculptData><FlexiSoftness>0</FlexiSoftness><FlexiTension>0</FlexiTension><FlexiDrag>0</FlexiDrag><FlexiGravity>0</FlexiGravity><FlexiWind>0</FlexiWind><FlexiForceX>0</FlexiForceX><FlexiForceY>0</FlexiForceY><FlexiForceZ>0</FlexiForceZ><LightColorR>0</LightColorR><LightColorG>0</LightColorG><LightColorB>0</LightColorB><LightColorA>1</LightColorA><LightRadius>0</LightRadius><LightCutoff>0</LightCutoff><LightFalloff>0</LightFalloff><LightIntensity>1</LightIntensity><FlexiEntry>false</FlexiEntry><LightEntry>false</LightEntry><SculptEntry>true</SculptEntry></Shape><Scale><X>1.417382</X><Y>1.2501</Y><Z>1.486959</Z></Scale><SitTargetOrientation><X>0</X><Y>0</Y><Z>0</Z><W>1</W></SitTargetOrientation><SitTargetPosition><X>0</X><Y>0</Y><Z>0</Z></SitTargetPosition><SitTargetPositionLL><X>0</X><Y>0</Y><Z>0</Z></SitTargetPositionLL><SitTargetOrientationLL><X>0</X><Y>0</Y><Z>0</Z><W>1</W></SitTargetOrientationLL><ParentID>0</ParentID><CreationDate>1357053677</CreationDate><Category>0</Category><SalePrice>0</SalePrice><ObjectSaleType>0</ObjectSaleType><OwnershipCost>0</OwnershipCost><GroupID><UUID>00000000-0000-0000-0000-000000000000</UUID></GroupID><OwnerID><UUID>47704d5f-910f-46ac-a685-7dcdf7bad9f3</UUID></OwnerID><LastOwnerID><UUID>47704d5f-910f-46ac-a685-7dcdf7bad9f3</UUID></LastOwnerID><BaseMask>647168</BaseMask><OwnerMask>647168</OwnerMask><GroupMask>0</GroupMask><EveryoneMask>0</EveryoneMask><NextOwnerMask>581632</NextOwnerMask><Flags>Phantom</Flags><CollisionSound><UUID>00000000-0000-0000-0000-000000000000</UUID></CollisionSound><CollisionSoundVolume>0</CollisionSoundVolume><TextureAnimation></TextureAnimation><ParticleSystem></ParticleSystem><PayPrice0>-2</PayPrice0><PayPrice1>-2</PayPrice1><PayPrice2>-2</PayPrice2><PayPrice3>-2</PayPrice3><PayPrice4>-2</PayPrice4></SceneObjectPart><OtherParts /></SceneObjectGroup>");
        }
    }
}
