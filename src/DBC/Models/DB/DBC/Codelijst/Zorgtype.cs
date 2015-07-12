﻿//begindatum|einddatum|code|groepcode|element|beschrijving|hierarchieniveau|selecteerbaar|sorteervolgorde|prestatiecodedeel|mutatie|branche_indicatie
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using System.Globalization;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Zorgtype : Codelijst.CodeTable
    {
        public int ZorgtypeID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String groepcode { get; set; }
        public String element { get; set; }
        public String beschrijving { get; set; }
        public int hierarchieniveau { get; set; }
        public int selecteerbaar { get; set; }
        public int sorteervolgorde { get; set; }
        [Column(TypeName = "char"), StringLength(3)]
        public String prestatiecodedeel { get; set; }
        public int? mutatie { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }
    }
    public sealed class ZorgtypeMap : CsvClassMap<Zorgtype>
    {
        public ZorgtypeMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.groepcode).Index(3);
            Map(m => m.element).Index(4);
            Map(m => m.beschrijving).Index(5);
            Map(m => m.hierarchieniveau).Index(6);
            Map(m => m.selecteerbaar).Index(7);
            Map(m => m.sorteervolgorde).Index(8);
            Map(m => m.prestatiecodedeel).Index(9);
            Map(m => m.mutatie).Index(10);
            Map(m => m.branche_indicatie).Index(11);
        }
    }
}