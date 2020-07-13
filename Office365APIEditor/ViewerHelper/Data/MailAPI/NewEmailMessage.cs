﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information. 

using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace Office365APIEditor.ViewerHelper.Data.MailAPI
{
    class NewEmailMessage
    {
        public MailAddressCollection ToRecipients;
        public MailAddressCollection CcRecipients;
        public MailAddressCollection BccRecipients;
        public string Subject;
        public BodyType BodyType;
        public string Body;
        public Importance Importance;
        public bool RequestDeliveryReceipt;
        public bool RequestReadReceipt;
        public bool SaveToSentItems;
        public List<Data.AttachmentAPI.AttachmentBase> Attachments;
    }

    public enum Importance
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    [DataContract]
    public enum BodyType
    {
        [EnumMember(Value = "text")]
        Text = 0,

        [EnumMember(Value = "html")]
        HTML = 1
    }
}