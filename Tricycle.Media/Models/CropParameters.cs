﻿using System;
namespace Tricycle.Media.Models
{
    public class CropParameters
    {
        public Coordinate<int> Start { get; set; }
        public Dimensions Size { get; set; }
    }
}