(()=>{
  const reducedMotion=window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  document.querySelectorAll('[data-mini-math]').forEach(shell=>{
    const card=shell.querySelector('.mini-math-card');
    const options=[...shell.querySelectorAll('[data-answer]')];
    const feedback=shell.querySelector('.mini-feedback');
    const retry=shell.querySelector('.mini-retry');
    const correctAnswer=shell.dataset.correctAnswer||'5';
    const successParts=(shell.dataset.success||'Harika! Doğru cevabı buldun.|Çözüm adımlarını doğru uyguladın.').split('|');
    const hintParts=(shell.dataset.hint||'Yaklaştın.|İşlem sırasını yeniden düşün.').split('|');
    const reset=()=>{
      options.forEach(button=>{button.disabled=false;button.classList.remove('correct','wrong')});
      feedback.className='mini-feedback';
      feedback.innerHTML='<span>Bir seçenek seç ve çözümünü kontrol et.</span>';
      retry.hidden=true;
    };
    options.forEach(button=>button.addEventListener('click',()=>{
      const correct=button.dataset.answer===correctAnswer;
      options.forEach(item=>item.disabled=true);
      button.classList.add(correct?'correct':'wrong');
      if(correct){
        feedback.className='mini-feedback success';
        feedback.innerHTML=`<strong>${successParts[0]}</strong><br>${successParts[1]||''}`;
        card.classList.add('celebrate');
      }else{
        feedback.className='mini-feedback hint';
        feedback.innerHTML=`<strong>${hintParts[0]}</strong><br>${hintParts[1]||''}`;
        retry.hidden=false;
      }
    }));
    retry.addEventListener('click',reset);
  });

  const gradeData={
    5:{label:'5. SINIF ROTASI',title:'Temeli sağlamlaştır, matematiği keşfet.',copy:'Doğal sayılar, kesirler ve geometrik şekilleri görsel çalışmalarla anlamlandır.',topics:['Kesirler','Doğal sayılar','Geometri','Ondalık gösterim'],duration:'45 dk',skill:'Matematiksel düşünme'},
    6:{label:'6. SINIF ROTASI',title:'Bağlantıları kur, işlem gücünü artır.',copy:'Oran, cebirsel ifadeler ve veri konularını günlük hayat örnekleriyle öğren.',topics:['Oran','Cebirsel ifadeler','Alan','Veri analizi'],duration:'50 dk',skill:'İlişki kurma'},
    7:{label:'7. SINIF ROTASI',title:'Problem çözme stratejilerini geliştir.',copy:'Denklemler, yüzdeler ve dönüşüm geometrisinde yeni nesil sorulara hazırlan.',topics:['Denklemler','Yüzdeler','Çember','Veri analizi'],duration:'55 dk',skill:'Stratejik çözüm'},
    8:{label:'8. SINIF · LGS ROTASI',title:'LGS için düşün, analiz et ve hızlan.',copy:'Yeni nesil sorular, düzenli denemeler ve konu analizleriyle sınava güvenle hazırlan.',topics:['Kareköklü ifadeler','Cebirsel ifadeler','Doğrusal denklemler','Olasılık'],duration:'60 dk',skill:'Analiz ve hız'}
  };
  document.querySelectorAll('[data-grade-explorer]').forEach(explorer=>{
    const dataNode=explorer.querySelector('.grade-explorer-data');
    if(dataNode){
      JSON.parse(dataNode.textContent).forEach(item=>{if(item.grade)gradeData[item.grade]={label:item.label,title:item.title,copy:item.copy,topics:item.topics,duration:item.duration,skill:item.skill}});
    }
    const label=explorer.querySelector('[data-grade-label]');
    const title=explorer.querySelector('[data-grade-title]');
    const copy=explorer.querySelector('[data-grade-copy]');
    const topics=explorer.querySelector('[data-grade-topics]');
    const duration=explorer.querySelector('[data-grade-duration]');
    const skill=explorer.querySelector('[data-grade-skill]');
    explorer.querySelectorAll('[data-explore-grade]').forEach(button=>button.addEventListener('click',()=>{
      const data=gradeData[button.dataset.exploreGrade];
      explorer.querySelectorAll('[data-explore-grade]').forEach(item=>{const active=item===button;item.classList.toggle('active',active);item.setAttribute('aria-selected',String(active))});
      label.textContent=data.label;title.textContent=data.title;copy.textContent=data.copy;duration.textContent=data.duration;skill.textContent=data.skill;
      topics.innerHTML=data.topics.map(topic=>`<b>${topic}</b>`).join('');
      const detail=explorer.querySelector('.grade-detail>div');detail.style.animation='none';void detail.offsetWidth;detail.style.animation='fadeUp .3s';
    }));
  });

  document.querySelectorAll('[data-roadmap]').forEach(roadmap=>{
    const dataNode=roadmap.querySelector('#roadmapData');
    if(!dataNode)return;
    const data=JSON.parse(dataNode.textContent);
    const number=roadmap.querySelector('[data-roadmap-number]');
    const title=roadmap.querySelector('[data-roadmap-title]');
    const copy=roadmap.querySelector('[data-roadmap-copy]');
    const line=roadmap.querySelector('.roadmap-line b');
    roadmap.querySelectorAll('[data-roadmap-step]').forEach(button=>button.addEventListener('click',()=>{
      const index=Number(button.dataset.roadmapStep);const item=data[index];
      roadmap.querySelectorAll('[data-roadmap-step]').forEach(step=>{const active=step===button;step.classList.toggle('active',active);step.setAttribute('aria-pressed',String(active))});
      number.textContent=item.number;title.textContent=item.title;copy.textContent=item.copy;
      if(window.innerWidth>700)line.style.width=`${12+index*29}%`;else line.style.height=`${12+index*29}%`;
    }));
  });

  if(!reducedMotion){
    document.querySelectorAll('[data-parallax-hero]').forEach(hero=>hero.addEventListener('pointermove',event=>{
      if(event.pointerType==='touch')return;
      const x=(event.clientX/window.innerWidth-.5)*10;const y=(event.clientY/window.innerHeight-.5)*10;
      hero.querySelectorAll('.mm-glow,.mm-float-symbol').forEach((layer,index)=>layer.style.transform=`translate3d(${x*(index+1)*.35}px,${y*(index+1)*.35}px,0)`);
    }));
  }

  window.bindContactForm=(serviceId,templateId)=>{
    const form=document.getElementById('contactForm'),status=document.getElementById('contactStatus');if(!form)return;
    form.addEventListener('submit',async event=>{event.preventDefault();const button=form.querySelector('button');button.disabled=true;status.className='contact-status sending';status.textContent='Mesajınız gönderiliyor…';try{await emailjs.sendForm(serviceId,templateId,form);status.className='contact-status success';status.textContent='Mesajınız başarıyla gönderildi.';form.reset()}catch{status.className='contact-status error';status.textContent='Mesaj gönderilemedi. Lütfen doğrudan e-posta ile iletişime geçin.'}finally{button.disabled=false}})
  };
})();
