using Microsoft.EntityFrameworkCore;
using Server_RPG.Models;

namespace Server_RPG.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1:1 관계로 명시적 설정 변경
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Character)   // 계정은 하나의 캐릭터를 가짐
                .WithOne(c => c.Account)    // 캐릭터도 하나의 계정에 속함
                .HasForeignKey<Character>(c => c.AccountID); // 외래키는 Character 테이블의 AccountID
        }
    }
}