﻿using System;
using JetBrains.Annotations;

namespace Core.States
{
    public interface IResponse : IOutcome
    {
        bool Generate();
    }
}