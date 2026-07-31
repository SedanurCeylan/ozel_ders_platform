(() => {
  const shell = document.querySelector('.arena-shell');
  if (!shell) return;
  let question = window.arenaQuestion;
  let questionStarted = performance.now();
  let busy = false;
  const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
  const end = Number(shell.dataset.end);
  const sessionId = Number(shell.dataset.session);
  const feedback = document.getElementById('feedback');
  const render = q => {
    question = q; questionStarted = performance.now(); busy = false;
    document.getElementById('questionNumber').textContent = `SORU ${q.number}`;
    document.getElementById('questionText').textContent = q.text;
    const area = document.getElementById('answerArea'); area.innerHTML = '';
    q.options.forEach(option => { const button = document.createElement('button'); button.type = 'button'; button.dataset.answer = option; button.textContent = option; area.appendChild(button); });
    feedback.className = ''; feedback.textContent = '';
  };
  const submit = async answer => {
    if (busy) return; busy = true;
    document.querySelectorAll('#answerArea button').forEach(x => x.disabled = true);
    try {
      const response = await fetch('/Student/Games/Answer', { method: 'POST', headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token }, body: JSON.stringify({ sessionId, questionId: question.id, answer, answerTimeSeconds: Math.max(.2, (performance.now() - questionStarted) / 1000) }) });
      if (!response.ok) { const e = await response.json(); throw new Error(e.message || 'Cevap kaydedilemedi.'); }
      const result = await response.json();
      const answerSeconds = (performance.now() - questionStarted) / 1000;
      const speedText = result.isCorrect && answerSeconds <= 3 ? ' ⚡ Süper hızlı!' : result.isCorrect && answerSeconds <= 6 ? ' ⚡ Hız bonusu!' : '';
      feedback.className = result.isCorrect ? 'correct' : 'wrong'; feedback.innerHTML = result.isCorrect ? `✓ Doğru! +${result.earnedScore} puan${speedText}` : `Doğru cevap: <strong>${result.correctAnswer}</strong><small>${result.explanation}</small>`;
      document.getElementById('score').textContent = result.totalScore; document.getElementById('correct').textContent = result.correct; document.getElementById('wrong').textContent = result.wrong; document.getElementById('streak').textContent = result.streak;
      const total = Math.max(1, result.correct + result.wrong); document.getElementById('studentProgress').style.width = `${Math.min(100, result.correct / total * 100)}%`; document.getElementById('opponentProgress').style.width = `${Math.min(92, 30 + total * 4)}%`;
      setTimeout(() => result.finished ? finish() : render(result.nextQuestion), 1200);
    } catch (e) { feedback.className = 'wrong'; feedback.textContent = e.message; setTimeout(finish, 900); }
  };
  const finish = () => location.href = `/Student/Games/Complete/${sessionId}`;
  document.getElementById('answerArea').addEventListener('click', e => { const button = e.target.closest('button[data-answer]'); if (button) submit(button.dataset.answer); });
  let finishing = false;
  const finishSafely = () => { if (finishing) return; finishing = true; finish(); };
  const tick = () => {
    if (!Number.isFinite(end) || end <= 0) { document.getElementById('timer').textContent = 'Hazır'; return; }
    const left = Math.max(0, Math.ceil((end - Date.now()) / 1000));
    document.getElementById('timer').textContent = `${left} sn`;
    if (left <= 0) finishSafely();
  };
  tick(); setInterval(tick, 500);
})();
