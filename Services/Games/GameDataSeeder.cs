using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Games;

namespace OzelDersYonetim.Services.Games;

public class GameDataSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync()
    {
        var questions = new List<GameQuestion>();
        if (!await db.GameQuestions.AnyAsync(x => x.GameType == GameType.OperationArena))
        {
        for (var grade = 5; grade <= 8; grade++)
        {
            var baseValue = grade * 3;
            questions.Add(Make(grade, "Dört İşlem", GameDifficulty.Easy, $"{baseValue} + {grade + 7} işleminin sonucu kaçtır?", (baseValue + grade + 7).ToString(), $"{baseValue} ile {grade + 7} toplanır.", baseValue + grade + 7));
            var divisor = grade - 3; var dividend = divisor * (grade + 4);
            questions.Add(Make(grade, "Dört İşlem", GameDifficulty.Medium, $"{dividend} ÷ {divisor} işleminin sonucu kaçtır?", (grade + 4).ToString(), "Bölünen sayı bölen sayıya ayrılır.", grade + 4));
            questions.Add(Make(grade, "Dört İşlem", GameDifficulty.Hard, $"({grade + 4} × {grade}) - {baseValue} işleminin sonucu kaçtır?", (((grade + 4) * grade) - baseValue).ToString(), "Önce parantez içindeki çarpma, sonra çıkarma yapılır.", ((grade + 4) * grade) - baseValue));
            var studentCount = grade - 2; var pensPerStudent = grade + 1;
            questions.Add(Make(grade, "Problemler", GameDifficulty.Medium, $"Bir kutuda {studentCount * pensPerStudent} kalem vardır. Kalemler {studentCount} öğrenciye eşit paylaştırılırsa kişi başına kaç kalem düşer?", pensPerStudent.ToString(), "Toplam kalem sayısı öğrenci sayısına bölünür.", pensPerStudent));
            questions.Add(Make(grade, grade >= 7 ? "Yüzdeler" : "Ondalık Sayılar", GameDifficulty.Medium, grade >= 7 ? "200 sayısının %25'i kaçtır?" : "2,5 + 1,5 işleminin sonucu kaçtır?", grade >= 7 ? "50" : "4", grade >= 7 ? "200 × 25 ÷ 100 = 50" : "Ondalık basamaklar hizalanarak toplanır.", grade >= 7 ? 50 : 4));
            questions.Add(Make(grade, grade == 8 ? "Üslü İfadeler" : "Dört İşlem", GameDifficulty.Hard, grade == 8 ? "2³ × 2² işleminin sonucu kaçtır?" : $"{grade * 10} sayısının yarısının {grade} fazlası kaçtır?", grade == 8 ? "32" : (grade * 6).ToString(), grade == 8 ? "Aynı tabanlı kuvvetlerde üsler toplanır: 2⁵ = 32." : "Önce sayının yarısı bulunur, sonra ekleme yapılır.", grade == 8 ? 32 : grade * 6));
            for (var number = 1; number <= 18; number++)
            {
                var left = grade * 7 + number * 2; var right = grade + number;
                var operation = number % 3;
                var correct = operation switch { 0 => left + right, 1 => left - right, _ => grade * (number + 2) };
                var text = operation switch { 0 => $"{left} + {right} işleminin sonucu kaçtır?", 1 => $"{left} - {right} işleminin sonucu kaçtır?", _ => $"{grade} × {number + 2} işleminin sonucu kaçtır?" };
                var difficulty = number <= 6 ? GameDifficulty.Easy : number <= 13 ? GameDifficulty.Medium : GameDifficulty.Hard;
                questions.Add(Make(grade, "Dört İşlem", difficulty, text, correct.ToString(), $"İşlem uygulandığında sonuç {correct} bulunur.", correct));
            }
        }
        }
        if (questions.Count > 0) { db.GameQuestions.AddRange(questions); await db.SaveChangesAsync(); }

        if (!await db.GameQuestions.AnyAsync(x => x.GameType == GameType.LevelTest))
        {
            var levelQuestions = new List<GameQuestion>();
            for (var grade = 5; grade <= 8; grade++)
            {
                var samples = new[]
                {
                    Make(grade,"Dört İşlem",GameDifficulty.Easy,$"{grade * 6} + {grade + 4} işleminin sonucu kaçtır?",(grade * 7 + 4).ToString(),"Sayılar toplanarak sonuç bulunur.",grade * 7 + 4),
                    Make(grade,"Dört İşlem",GameDifficulty.Medium,$"{grade * 8} ÷ {grade} işleminin sonucu kaçtır?","8","Bölme işlemi uygulandığında sonuç 8 olur.",8),
                    Make(grade,"Problemler",GameDifficulty.Medium,$"Her birinde {grade} kalem bulunan 4 kutuda toplam kaç kalem vardır?",(grade * 4).ToString(),"Kutu sayısı ile her kutudaki kalem sayısı çarpılır.",grade * 4),
                    Make(grade,grade>=7?"Yüzdeler":"Kesirler",GameDifficulty.Medium,grade>=7?"100 sayısının %20'si kaçtır?":"Bir bütünün yarısı kaç eş parçadan biridir?",grade>=7?"20":"2",grade>=7?"100 × 20 ÷ 100 = 20":"Yarım, bütünün iki eş parçasından biridir.",grade>=7?20:2),
                    Make(grade,grade==8?"Üslü İfadeler":"Dört İşlem",GameDifficulty.Hard,grade==8?"2⁴ işleminin sonucu kaçtır?":$"({grade}+3) × 2 işleminin sonucu kaçtır?",grade==8?"16":((grade+3)*2).ToString(),grade==8?"2 × 2 × 2 × 2 = 16":"Önce parantez, sonra çarpma yapılır.",grade==8?16:(grade+3)*2)
                };
                foreach (var question in samples) { question.GameType = GameType.LevelTest; question.TimeLimitSeconds = 60; }
                levelQuestions.AddRange(samples);
            }
            db.GameQuestions.AddRange(levelQuestions);
            await db.SaveChangesAsync();
        }
    }
    private static GameQuestion Make(int grade, string topic, GameDifficulty difficulty, string text, string answer, string explanation, int correct)
    {
        var values = new[] { correct, correct + 2, Math.Max(0, correct - 3), correct + 5 }.Distinct().Take(4).OrderBy(_ => Guid.NewGuid()).ToArray();
        var q = new GameQuestion { GradeLevel = grade, Topic = topic, DifficultyLevel = difficulty, QuestionType = GameQuestionType.MultipleChoice, QuestionText = text, CorrectAnswer = answer, Explanation = explanation, Score = difficulty == GameDifficulty.Hard ? 150 : difficulty == GameDifficulty.Medium ? 120 : 100, TimeLimitSeconds = difficulty == GameDifficulty.Hard ? 15 : 20 };
        q.Options = values.Select((x, i) => new GameQuestionOption { OptionText = x.ToString(), IsCorrect = x.ToString() == answer, DisplayOrder = i }).ToList(); return q;
    }
}
