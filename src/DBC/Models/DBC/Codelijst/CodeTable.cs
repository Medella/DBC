﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Entity;

namespace DBC.Models.DB.DBC.Codelijst
{
    public class CodeTable
    {
        //[Index("IX_Updatekey", 0, IsUnique = true)]
        public DateTime begindatum { get; set; }
        public DateTime einddatum { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String Code { get; set; }
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CodeTable>(b =>
            {
                b.Property(c => c.Code).HasColumnType("varchar").HasMaxLength(20);
                b.HasIndex(p => p.Code).IsUnique(true);
            });
        }
    }
}