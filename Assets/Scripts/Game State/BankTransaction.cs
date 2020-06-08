﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable, DataContract]
public struct BankTransaction
{
    [DataMember(IsRequired = true)]
    public int InitialCurrency, DeltaCurrency;
    [DataMember(IsRequired = true)]
    public string Description;

    [SerializeField]
    string dateString;
    [DataMember(IsRequired = true)]
    public DateTime Date
    {
        get => DateTime.Parse(dateString);
        set => dateString = value.ToString();
    }
}