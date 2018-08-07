namespace AutenticacaoAspNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriaTipoUsuario : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Login = c.String(nullable: false, maxLength: 50),
                        Senha = c.String(nullable: false, maxLength: 100),
                        Tipo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuarios");
        }
    }
}
