using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace sors.Migrations
{
    public partial class Reset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.CreateTable(
            //        name: "ActivityTypes",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true),
            //            ParentId = table.Column<int>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_ActivityTypes", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_ActivityTypes_ActivityTypes_ParentId",
            //                column: x => x.ParentId,
            //                principalTable: "ActivityTypes",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "BusinessProcesses",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true),
            //            ParentId = table.Column<int>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_BusinessProcesses", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_BusinessProcesses_BusinessProcesses_ParentId",
            //                column: x => x.ParentId,
            //                principalTable: "BusinessProcesses",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "BusinessTypes",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_BusinessTypes", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Departments",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true),
            //            ShortName = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Departments", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Incidents",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            DateCreate = table.Column<DateTime>(type: "date", nullable: false),
            //            DateIncident = table.Column<DateTime>(type: "date", nullable: false),
            //            Description = table.Column<string>(nullable: true),
            //            Status = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Incidents", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskAreas",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskAreas", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskDurations",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskDurations", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskFactors",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Code = table.Column<int>(nullable: false),
            //            Name = table.Column<string>(nullable: true),
            //            ParentId = table.Column<int>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskFactors", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_RiskFactors_RiskFactors_ParentId",
            //                column: x => x.ParentId,
            //                principalTable: "RiskFactors",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskManageabilities",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskManageabilities", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskReactions",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskReactions", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskSignificances",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskSignificances", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RiskStatuses",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RiskStatuses", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Roles",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Roles", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "TextDatas",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true),
            //            Value = table.Column<string>(nullable: true),
            //            Param = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_TextDatas", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Accounts",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true),
            //            DepartmentId = table.Column<int>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Accounts", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Accounts_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "DomainDepartments",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(nullable: true),
            //            DepartmentId = table.Column<int>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_DomainDepartments", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_DomainDepartments_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Responsibles",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            IncidentId = table.Column<int>(nullable: false),
            //            DepartmentId = table.Column<int>(nullable: false),
            //            Result = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Responsibles", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Responsibles_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_Responsibles_Incidents_IncidentId",
            //                column: x => x.IncidentId,
            //                principalTable: "Incidents",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AccountRoles",
            //        columns: table => new
            //        {
            //            AccountId = table.Column<int>(nullable: false),
            //            RoleId = table.Column<int>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AccountRoles", x => new { x.AccountId, x.RoleId });
            //            table.ForeignKey(
            //                name: "FK_AccountRoles_Accounts_AccountId",
            //                column: x => x.AccountId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_AccountRoles_Roles_RoleId",
            //                column: x => x.RoleId,
            //                principalTable: "Roles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Drafts",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            DateCreate = table.Column<DateTime>(type: "date", nullable: false),
            //            Description1 = table.Column<string>(nullable: true),
            //            Description2 = table.Column<string>(nullable: true),
            //            Description3 = table.Column<string>(nullable: true),
            //            Description4 = table.Column<string>(nullable: true),
            //            Description5 = table.Column<string>(nullable: true),
            //            AuthorId = table.Column<int>(nullable: false),
            //            DepartmentId = table.Column<int>(nullable: false),
            //            Status = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Drafts", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Drafts_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_Drafts_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "IncidentProp",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            IncidentId = table.Column<int>(nullable: false),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            AuthorId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Comment = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_IncidentProp", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_IncidentProp_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_IncidentProp_Incidents_IncidentId",
            //                column: x => x.IncidentId,
            //                principalTable: "Incidents",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Logs",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            AccountId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Info = table.Column<string>(nullable: true),
            //            Timestamp = table.Column<DateTime>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Logs", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Logs_Accounts_AccountId",
            //                column: x => x.AccountId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Measures",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            DateCreate = table.Column<DateTime>(type: "date", nullable: false),
            //            ResponsibleId = table.Column<int>(nullable: false),
            //            Description = table.Column<string>(nullable: true),
            //            ExpectedResult = table.Column<string>(nullable: true),
            //            DeadLine = table.Column<DateTime>(type: "date", nullable: true),
            //            DeadLineText = table.Column<string>(nullable: true),
            //            Status = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Measures", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Measures_Responsibles_ResponsibleId",
            //                column: x => x.ResponsibleId,
            //                principalTable: "Responsibles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "ResponsibleAccounts",
            //        columns: table => new
            //        {
            //            AccountId = table.Column<int>(nullable: false),
            //            ResponsibleId = table.Column<int>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_ResponsibleAccounts", x => new { x.ResponsibleId, x.AccountId });
            //            table.ForeignKey(
            //                name: "FK_ResponsibleAccounts_Accounts_AccountId",
            //                column: x => x.AccountId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_ResponsibleAccounts_Responsibles_ResponsibleId",
            //                column: x => x.ResponsibleId,
            //                principalTable: "Responsibles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "ResponsibleProp",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            ResponsibleId = table.Column<int>(nullable: false),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            AuthorId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Comment = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_ResponsibleProp", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_ResponsibleProp_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_ResponsibleProp_Responsibles_ResponsibleId",
            //                column: x => x.ResponsibleId,
            //                principalTable: "Responsibles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "DraftProp",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            DraftId = table.Column<int>(nullable: false),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            AuthorId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Comment = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_DraftProp", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_DraftProp_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_DraftProp_Drafts_DraftId",
            //                column: x => x.DraftId,
            //                principalTable: "Drafts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "IncidentDrafts",
            //        columns: table => new
            //        {
            //            IncidentId = table.Column<int>(nullable: false),
            //            DraftId = table.Column<int>(nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_IncidentDrafts", x => new { x.IncidentId, x.DraftId });
            //            table.ForeignKey(
            //                name: "FK_IncidentDrafts_Drafts_DraftId",
            //                column: x => x.DraftId,
            //                principalTable: "Drafts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_IncidentDrafts_Incidents_IncidentId",
            //                column: x => x.IncidentId,
            //                principalTable: "Incidents",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "MeasureProp",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            MeasureId = table.Column<int>(nullable: false),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            AuthorId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Comment = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_MeasureProp", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_MeasureProp_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_MeasureProp_Measures_MeasureId",
            //                column: x => x.MeasureId,
            //                principalTable: "Measures",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Reports",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            MeasureId = table.Column<int>(nullable: false),
            //            Description = table.Column<string>(nullable: true),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            Status = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Reports", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Reports_Measures_MeasureId",
            //                column: x => x.MeasureId,
            //                principalTable: "Measures",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "ReportProp",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            ReportId = table.Column<int>(nullable: false),
            //            DateCreate = table.Column<DateTime>(nullable: false),
            //            AuthorId = table.Column<int>(nullable: false),
            //            Action = table.Column<string>(nullable: true),
            //            Comment = table.Column<string>(nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_ReportProp", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_ReportProp_Accounts_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Accounts",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_ReportProp_Reports_ReportId",
            //                column: x => x.ReportId,
            //                principalTable: "Reports",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AccountRoles_RoleId",
            //        table: "AccountRoles",
            //        column: "RoleId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Accounts_DepartmentId",
            //        table: "Accounts",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ActivityTypes_ParentId",
            //        table: "ActivityTypes",
            //        column: "ParentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_BusinessProcesses_ParentId",
            //        table: "BusinessProcesses",
            //        column: "ParentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_DomainDepartments_DepartmentId",
            //        table: "DomainDepartments",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_DraftProp_AuthorId",
            //        table: "DraftProp",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_DraftProp_DraftId",
            //        table: "DraftProp",
            //        column: "DraftId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Drafts_AuthorId",
            //        table: "Drafts",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Drafts_DepartmentId",
            //        table: "Drafts",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_IncidentDrafts_DraftId",
            //        table: "IncidentDrafts",
            //        column: "DraftId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_IncidentProp_AuthorId",
            //        table: "IncidentProp",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_IncidentProp_IncidentId",
            //        table: "IncidentProp",
            //        column: "IncidentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Logs_AccountId",
            //        table: "Logs",
            //        column: "AccountId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_MeasureProp_AuthorId",
            //        table: "MeasureProp",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_MeasureProp_MeasureId",
            //        table: "MeasureProp",
            //        column: "MeasureId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Measures_ResponsibleId",
            //        table: "Measures",
            //        column: "ResponsibleId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ReportProp_AuthorId",
            //        table: "ReportProp",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ReportProp_ReportId",
            //        table: "ReportProp",
            //        column: "ReportId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Reports_MeasureId",
            //        table: "Reports",
            //        column: "MeasureId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ResponsibleAccounts_AccountId",
            //        table: "ResponsibleAccounts",
            //        column: "AccountId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ResponsibleProp_AuthorId",
            //        table: "ResponsibleProp",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ResponsibleProp_ResponsibleId",
            //        table: "ResponsibleProp",
            //        column: "ResponsibleId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Responsibles_DepartmentId",
            //        table: "Responsibles",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Responsibles_IncidentId",
            //        table: "Responsibles",
            //        column: "IncidentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RiskFactors_ParentId",
            //        table: "RiskFactors",
            //        column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "BusinessProcesses");

            migrationBuilder.DropTable(
                name: "BusinessTypes");

            migrationBuilder.DropTable(
                name: "DomainDepartments");

            migrationBuilder.DropTable(
                name: "DraftProp");

            migrationBuilder.DropTable(
                name: "IncidentDrafts");

            migrationBuilder.DropTable(
                name: "IncidentProp");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MeasureProp");

            migrationBuilder.DropTable(
                name: "ReportProp");

            migrationBuilder.DropTable(
                name: "ResponsibleAccounts");

            migrationBuilder.DropTable(
                name: "ResponsibleProp");

            migrationBuilder.DropTable(
                name: "RiskAreas");

            migrationBuilder.DropTable(
                name: "RiskDurations");

            migrationBuilder.DropTable(
                name: "RiskFactors");

            migrationBuilder.DropTable(
                name: "RiskManageabilities");

            migrationBuilder.DropTable(
                name: "RiskReactions");

            migrationBuilder.DropTable(
                name: "RiskSignificances");

            migrationBuilder.DropTable(
                name: "RiskStatuses");

            migrationBuilder.DropTable(
                name: "TextDatas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Measures");

            migrationBuilder.DropTable(
                name: "Responsibles");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}
