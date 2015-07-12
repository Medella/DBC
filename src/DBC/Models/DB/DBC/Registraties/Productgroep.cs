﻿//begindatum|einddatum|code|code_verblijf|code_behandeling|type|omschrijving_verblijf|omschrijving_behandeling|beschrijving|hierarchieniveau|selecteerbaar|sorteervolgorde|
//setting|categorie|lekenvertaling|diagnose_blinderen|mutatie|branche_indicatie
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.Data.Entity;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Productgroep : Codelijst.CodeTable
    {
        public int ProductgroepID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), Column(TypeName = "varchar"), StringLength(6)]
        new public String Code { get; set; }
        [Column(TypeName = "char"), StringLength(3)]
        public String code_verblijf { get; set; }
        [Column(TypeName = "char"), StringLength(3)]
        public String code_behandeling { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String type { get; set; }
        public String omschrijving_verblijf { get; set; }
        public String omschrijving_behandeling { get; set; }
        public String beschrijving { get; set; }
        public int hierarchieniveau { get; set; }
        public int selecteerbaar { get; set; }
        public int sorteervolgorde { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String setting { get; set; }
        [Column(TypeName = "varchar"), StringLength(50)]
        public String categorie { get; set; }
        public String lekenvertaling { get; set; }
        [Column(TypeName = "char"),StringLength(1)]
        public String diagnose_blinderen { get; set; }
        public int? mutatie { get; set; }
        public int branche_indicatie { get; set; }
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Productgroep>(b =>
            {
                b.Property(c => c.code_verblijf).ColumnType("char").MaxLength(3);
                b.Property(c => c.code_behandeling).ColumnType("char").MaxLength(3);
                b.Property(c => c.type).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.setting).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.categorie).ColumnType("char").MaxLength(50);
                b.Property(c => c.diagnose_blinderen).ColumnType("char").MaxLength(1);
                b.Index(p => p.Code).Unique(true);
            });
        }
    }
    public sealed class ProductgroepMap : CsvClassMap<Productgroep>
    {
        public ProductgroepMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.code_verblijf).Index(3);
            Map(m => m.code_behandeling).Index(4);
            Map(m => m.type).Index(5);
            Map(m => m.omschrijving_verblijf).Index(6);
            Map(m => m.omschrijving_behandeling).Index(7);
            Map(m => m.beschrijving).Index(8);
            Map(m => m.hierarchieniveau).Index(9);
            Map(m => m.selecteerbaar).Index(10);
            Map(m => m.sorteervolgorde).Index(11);
            Map(m => m.setting).Index(12);
            Map(m => m.categorie).Index(13);
            Map(m => m.lekenvertaling).Index(14);
            Map(m => m.diagnose_blinderen ).Index(15);
            Map(m => m.mutatie).Index(16);
            Map(m => m.branche_indicatie).Index(17);
        }
    }
}