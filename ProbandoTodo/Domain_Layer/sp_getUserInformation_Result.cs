//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Domain_Layer
{
    using System;
    
    public partial class sp_getUserInformation_Result
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public string PersonalPhrase { get; set; }
        public string PhraseColor { get; set; }
        public byte[] AvatarImage { get; set; }
        public string AvatarMIMEType { get; set; }
    }
}
