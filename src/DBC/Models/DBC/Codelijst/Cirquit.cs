﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using Microsoft.Data.Entity;

namespace DBC.Models.DB.DBC.Codelijst
{
    public class Cirquit : Codelijst.CodeTable
    {
        public int CirquitID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), 
        [Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }
        public String beschrijving { get; set; }
        public int sorteervolgorde { get; set; }
        public int? mutatie { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }
        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Cirquit>(b =>
            {
                b.Property(c => c.Code).HasColumnType("varchar").HasMaxLength(20);
                b.HasIndex(p => new { p.Code, p.branche_indicatie }).IsUnique(true);
            });
        }
    }
    public sealed class CirquitMap : CsvClassMap<Cirquit>
    {
        public CirquitMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.beschrijving).Index(3);
            Map(m => m.sorteervolgorde).Index(4);
            Map(m => m.mutatie).Index(5);
            Map(m => m.branche_indicatie).Index(6);
        }
    }
}