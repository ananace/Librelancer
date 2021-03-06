﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Linq;
using System.Numerics;
using LibreLancer.GameData.Items;
namespace LibreLancer
{
    public class WeaponComponent : GameComponent
    {
        public GunEquipment Object;
        public double CurrentCooldown = 0;

        public WeaponComponent(GameObject parent, GunEquipment def) : base(parent)
        {
            Object = def;
        }

        public override void FixedUpdate(TimeSpan time)
        {
            CurrentCooldown -= time.TotalSeconds;
            if (CurrentCooldown < 0) CurrentCooldown = 0;
        }

        public void AimTowards(Vector3 point, TimeSpan time)
        {
            var hp = Parent.Attachment;
            //Parent is the gun itself rotated
            var beforeRotate = hp.TransformNoRotate * Parent.Parent.GetTransform();
            //Inverse Transform
            Matrix4x4.Invert(beforeRotate, out beforeRotate);
            var local = Vector3.Transform(point, beforeRotate);
            var localProper = local.Normalized();
            var rads = MathHelper.DegreesToRadians(Object.Def.TurnRate);
            var delta = (float)(time.TotalSeconds * rads);
            if(hp.Revolute != null) {
                var target = localProper.X * (float)Math.PI;
                var current = Parent.Attachment.CurrentRevolution;

                if(current > target) {
                    current -= delta;
                    if (current <= target) current = target;
                }
                if(current < target) {
                    current += delta;
                    if (current >= target) current = target;
                }
                hp.Revolve(current);
            }
            //TODO: Finding barrel construct properly?
            Utf.RevConstruct barrel = null;
            foreach (var mdl in Parent.RigidModel.AllParts)
                if (mdl.Construct is Utf.RevConstruct revCon)
                    barrel = revCon;
            if(barrel != null) {
                var target = -localProper.Y * (float)Math.PI;
                var current = barrel.Current;
                if (current > target)
                {
                    current -= delta;
                    if (current <= target) current = target;
                }
                if (current < target)
                {
                    current += delta;
                    if (current >= target) current = target;
                }

                barrel.Update(target, Quaternion.Identity);
                Parent.RigidModel.UpdateTransform();
            }
        }


        ProjectileManager projectiles;
        ProjectileData toSpawn;
        Hardpoint[] hpfires;

        public void Fire(Vector3 point)
        {
            if (CurrentCooldown > 0) return;
            if (projectiles == null) {
                hpfires = Parent.GetHardpoints().Where((x) => x.Name.StartsWith("hpfire", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                projectiles = Parent.GetWorld().Projectiles;
                toSpawn = projectiles.GetData(Object);
            }
            var tr = (Parent.Attachment.Transform * Parent.Parent.GetTransform());
            for (int i = 0; i < hpfires.Length; i++)
            {
                var pos = Vector3.Transform(Vector3.Zero, hpfires[i].Transform * tr);
                var heading = (pos - point).Normalized();
                projectiles.SpawnProjectile(toSpawn, pos, heading);
            }
            CurrentCooldown = Object.Def.RefireDelay;
        }
    }
}
