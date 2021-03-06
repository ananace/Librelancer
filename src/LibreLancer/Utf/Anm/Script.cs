﻿// MIT License - Copyright (c) Malte Rupprecht
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package


using System;
using System.Collections.Generic;

namespace LibreLancer.Utf.Anm
{
    public class Script
    {
        public string Name { get; private set; }
        public bool HasRootHeight { get; private set; }
        public float RootHeight { get; private set; }
		public List<ObjectMap> ObjectMaps { get; private set; }
		public List<JointMap> JointMaps { get; private set; }

        public Script(IntermediateNode root, ConstructCollection constructs)
        {
            Name = root.Name;
			ObjectMaps = new List<ObjectMap>();
			JointMaps = new List<JointMap>();
            foreach (Node node in root)
            {
                if (node.Name.Equals("root height", StringComparison.OrdinalIgnoreCase))
                {
                    HasRootHeight = true;
                    RootHeight = (node as LeafNode).SingleData.Value;
                }
				else if (node.Name.StartsWith("object map", StringComparison.OrdinalIgnoreCase))
					ObjectMaps.Add(new ObjectMap(node as IntermediateNode));
				else if (node.Name.StartsWith("joint map", StringComparison.OrdinalIgnoreCase))
                    JointMaps.Add(new JointMap(node as IntermediateNode));
                else
                {
                    FLLog.Warning("Anm", $"{root.Name}: invalid node {node.Name}, possible broken animation?");
                }
            }
        }
    }
}
