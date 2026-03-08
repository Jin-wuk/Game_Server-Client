using Microsoft.EntityFrameworkCore;
using Server_RPG.Models;

namespace Server_RPG.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<MonsterSpawner> MonsterSpawners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1:1 관계로 명시적 설정 변경
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Character)   // 계정은 하나의 캐릭터를 가짐
                .WithOne(c => c.Account)    // 캐릭터도 하나의 계정에 속함
                .HasForeignKey<Character>(c => c.AccountID); // 외래키는 Character 테이블의 AccountID

            // 몬스터 테이블 이름을 'monster'로 강제 지정
            modelBuilder.Entity<Monster>().ToTable("monster");
            // 스포너 테이블 이름도 'monster_spawner'로 강제 지정
            modelBuilder.Entity<MonsterSpawner>().ToTable("monster_spawner");
        }
    }
}
    
