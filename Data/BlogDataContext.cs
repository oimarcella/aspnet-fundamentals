using Blog.Data.Mappings;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContext : DbContext
    {
        //Só vai receber ContextOptions desse dataContext e para esse DataContext
        public BlogDataContext(DbContextOptions<BlogDataContext> options)
        : base(options)//passando as options para nossa classe pai DbContext
        {
            //alem da connection string, posso dizer que uso sqlserver, quero exec migration de x forma
        }

        //Tabelas referenciadas
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        //Vai passar a receber a connection string do construtor, e tbm outras configuracoes
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //=> options.UseSqlServer("stringconnection");

        //Aplicando FluentMapping
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}