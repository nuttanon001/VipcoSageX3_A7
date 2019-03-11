﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VipcoSageX3.Models.SageX3Extends;

namespace VipcoSageX3.Migrations
{
    [DbContext(typeof(SageX3ExtendContext))]
    partial class SageX3ExtendContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VipcoSageX3.Models.SageX3Extends.ReceiptExtend", b =>
                {
                    b.Property<int>("ReceiptExtendId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<int?>("EndRange");

                    b.Property<DateTime?>("GetDate");

                    b.Property<string>("GetTime")
                        .HasMaxLength(10);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<int?>("StartRange");

                    b.HasKey("ReceiptExtendId");

                    b.ToTable("ReceiptExtend");
                });

            modelBuilder.Entity("VipcoSageX3.Models.SageX3Extends.TaskStatusDetail", b =>
                {
                    b.Property<int>("TaskStatusDetailId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Email")
                        .HasMaxLength(250);

                    b.Property<string>("EmployeeCode")
                        .HasMaxLength(20);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<int?>("TaskStatusMasterId");

                    b.HasKey("TaskStatusDetailId");

                    b.HasIndex("TaskStatusMasterId");

                    b.ToTable("TaskStatusDetail");
                });

            modelBuilder.Entity("VipcoSageX3.Models.SageX3Extends.TaskStatusMaster", b =>
                {
                    b.Property<int>("TaskStatusMasterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<string>("WorkGroupCode")
                        .HasMaxLength(50);

                    b.Property<string>("WorkGroupName")
                        .HasMaxLength(200);

                    b.HasKey("TaskStatusMasterId");

                    b.HasIndex("WorkGroupCode")
                        .IsUnique()
                        .HasFilter("[WorkGroupCode] IS NOT NULL");

                    b.ToTable("TaskStatusMaster");
                });

            modelBuilder.Entity("VipcoSageX3.Models.SageX3Extends.TaskStatusDetail", b =>
                {
                    b.HasOne("VipcoSageX3.Models.SageX3Extends.TaskStatusMaster", "TaskStatusMaster")
                        .WithMany("TaskStatusDetails")
                        .HasForeignKey("TaskStatusMasterId");
                });
#pragma warning restore 612, 618
        }
    }
}
