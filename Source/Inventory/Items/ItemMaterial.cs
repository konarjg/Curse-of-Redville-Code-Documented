﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material Item", menuName = "Curse of Redville/Items/Material")]
//Materiał do craftowania, nie ma żadnego użycia
public class ItemMaterial : Item
{
    public override bool Use()
    {
        return true;
    }
}
