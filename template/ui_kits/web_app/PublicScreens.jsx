/* global React, I, Btn, Badge, Card */
const { useState } = React;

// =================== Landing Page ===================
const Landing = ({ onLogin, onRegister, onLookup }) => (
  <div style={{background:'#fff', minHeight:'100vh'}} data-screen-label="Public · Landing">
    <header style={{position:'sticky', top:0, background:'rgba(255,255,255,0.95)', backdropFilter:'blur(8px)', borderBottom:'1px solid #E5E7EB', zIndex:10}}>
      <div style={{maxWidth:1200, margin:'0 auto', padding:'14px 32px', display:'flex', alignItems:'center', gap:32}}>
        <img src="../../assets/logo.svg" alt="Taalam" style={{height:28}}/>
        <nav style={{display:'flex', gap:24, flex:1}}>
          {['Home','Courses','About','Contact'].map(l => (
            <a key={l} href="#" style={{font:'500 14px/20px Inter, sans-serif', color:'#1A1A2E', textDecoration:'none'}}>{l}</a>
          ))}
          <a href="#" onClick={e=>{e.preventDefault(); onLookup();}} style={{font:'500 14px/20px Inter, sans-serif', color:'#0F766E', textDecoration:'none'}}>Verify certificate</a>
        </nav>
        <div style={{display:'flex', gap:10}}>
          <Btn variant="ghost" size="sm" onClick={onLogin}>Sign in</Btn>
          <Btn size="sm" onClick={onRegister}>Get started</Btn>
        </div>
      </div>
    </header>

    <section style={{maxWidth:1200, margin:'0 auto', padding:'72px 32px 56px', display:'grid', gridTemplateColumns:'1.1fr 1fr', gap:48, alignItems:'center'}}>
      <div>
        <div style={{display:'inline-flex', alignItems:'center', gap:8, padding:'4px 12px', borderRadius:9999, background:'#CCFBF1', color:'#0F766E', font:'600 12px/16px Inter, sans-serif', marginBottom:18}}>
          <span style={{width:6, height:6, borderRadius:999, background:'#0D9488'}}/> Bahrain's professional training platform
        </div>
        <h1 style={{font:'700 56px/64px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.02em', margin:0, textWrap:'balance'}}>Your path to professional certification.</h1>
        <p style={{font:'400 18px/28px Inter, sans-serif', color:'#6B7280', marginTop:16, maxWidth:520}}>
          Industry-recognized training across project management, IT, finance and safety — designed for professionals across the Kingdom of Bahrain.
        </p>
        <div style={{display:'flex', gap:12, marginTop:28}}>
          <Btn onClick={onRegister}>Browse courses</Btn>
          <Btn variant="secondary" onClick={onRegister}>Get started</Btn>
        </div>
      </div>
      <div style={{position:'relative', aspectRatio:'4/3', borderRadius:16, background:'linear-gradient(135deg, #0D9488, #0F766E)', overflow:'hidden', boxShadow:'0 20px 40px rgba(13,148,136,0.25)'}}>
        <div style={{position:'absolute', inset:0, padding:32, color:'#fff'}}>
          <div style={{display:'flex', justifyContent:'space-between', marginBottom:20}}>
            <div style={{font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', opacity:0.85}}>Live · 12 May 2026</div>
            <div style={{font:'600 11px/14px Inter, sans-serif', opacity:0.85}}>Manama · HQ</div>
          </div>
          <div style={{font:'700 24px/32px Inter, sans-serif', marginBottom:8}}>PMP-100 in session</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', opacity:0.9, marginBottom:24}}>Project Management Fundamentals · Fatima Khalifa</div>
          <div style={{display:'grid', gridTemplateColumns:'1fr 1fr', gap:12}}>
            {[{l:'Trainees', v:'14 / 20'},{l:'Hours', v:'18 / 24'},{l:'Avg score', v:'88%'},{l:'Next', v:'14:00'}].map(s=>(
              <div key={s.l} style={{padding:'12px 14px', background:'rgba(255,255,255,0.12)', borderRadius:10, backdropFilter:'blur(4px)'}}>
                <div style={{font:'500 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', opacity:0.8}}>{s.l}</div>
                <div style={{font:'700 22px/28px Inter, sans-serif', fontVariantNumeric:'tabular-nums', marginTop:2}}>{s.v}</div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>

    <section style={{maxWidth:1200, margin:'0 auto', padding:'24px 32px'}}>
      <div style={{display:'grid', gridTemplateColumns:'repeat(4,1fr)', gap:0, padding:'28px 0', borderTop:'1px solid #EEF0F2', borderBottom:'1px solid #EEF0F2'}}>
        {[{n:'42',l:'Active courses'},{n:'2,800+',l:'Certified trainees'},{n:'28',l:'Expert instructors'},{n:'12',l:'Certification tracks'}].map((s,i)=>(
          <div key={i} style={{textAlign:'center', borderRight: i<3?'1px solid #EEF0F2':'0'}}>
            <div style={{font:'700 32px/40px Inter, sans-serif', color:'#0F766E', letterSpacing:'-0.01em', fontVariantNumeric:'tabular-nums'}}>{s.n}</div>
            <div style={{font:'500 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:4}}>{s.l}</div>
          </div>
        ))}
      </div>
    </section>

    <section style={{maxWidth:1200, margin:'0 auto', padding:'56px 32px'}}>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'baseline', marginBottom:24}}>
        <h2 style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', margin:0}}>Featured courses</h2>
        <a href="#" style={{font:'500 14px/22px Inter, sans-serif', color:'#0F766E', textDecoration:'none'}}>Browse all →</a>
      </div>
      <div style={{display:'grid', gridTemplateColumns:'repeat(3,1fr)', gap:16}}>
        {[
          { code:'PMP-100', title:'Project Management Fundamentals', area:'Business', hrs:24, fee:120 },
          { code:'ITIL-300', title:'ITIL 4 Foundation', area:'IT', hrs:32, fee:240 },
          { code:'OSH-200', title:'Occupational Safety, Level 2', area:'Safety', hrs:16, fee:85 },
        ].map(c => (
          <Card key={c.code}>
            <div style={{font:'500 12px/16px Inter, sans-serif', color:'#0F766E', letterSpacing:'0.04em'}}>{c.code} · {c.area}</div>
            <div style={{font:'600 18px/26px Inter, sans-serif', color:'#1A1A2E', marginTop:6}}>{c.title}</div>
            <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginTop:14, paddingTop:14, borderTop:'1px solid #EEF0F2'}}>
              <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280'}}>{c.hrs} hrs · BHD {c.fee.toFixed(3)}</div>
              <Btn size="sm" variant="secondary" onClick={onRegister}>Enroll</Btn>
            </div>
          </Card>
        ))}
      </div>
    </section>

    <section style={{background:'#F8F9FA', padding:'56px 32px'}}>
      <div style={{maxWidth:1200, margin:'0 auto'}}>
        <h2 style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', margin:'0 0 8px', textAlign:'center'}}>How Taalam works</h2>
        <p style={{font:'400 15px/24px Inter, sans-serif', color:'#6B7280', textAlign:'center', margin:'0 0 32px'}}>Three steps from enrollment to certification.</p>
        <div style={{display:'grid', gridTemplateColumns:'repeat(3,1fr)', gap:16}}>
          {[
            { n:'01', ic:I.book, t:'Browse', d:'Explore courses across business, IT, safety, and finance — taught by certified instructors.' },
            { n:'02', ic:I.cal, t:'Enroll', d:'Pick a session that fits your schedule. Pay online and receive an instant confirmation.' },
            { n:'03', ic:I.award, t:'Get certified', d:'Pass your assessment and earn a verifiable certificate recognized across Bahrain.' },
          ].map((s,i) => (
            <div key={i} style={{background:'#fff', borderRadius:12, border:'1px solid #E5E7EB', padding:24}}>
              <div style={{display:'flex', alignItems:'center', gap:12, marginBottom:14}}>
                <div style={{width:48, height:48, borderRadius:12, background:'#CCFBF1', color:'#0F766E', display:'flex', alignItems:'center', justifyContent:'center'}}>{s.ic}</div>
                <div style={{font:'700 22px/28px Inter, sans-serif', color:'#0F766E', fontVariantNumeric:'tabular-nums'}}>{s.n}</div>
              </div>
              <div style={{font:'600 18px/26px Inter, sans-serif', color:'#1A1A2E'}}>{s.t}</div>
              <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:6}}>{s.d}</div>
            </div>
          ))}
        </div>
      </div>
    </section>

    <section style={{maxWidth:1200, margin:'0 auto', padding:'56px 32px'}}>
      <h2 style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', margin:'0 0 24px'}}>Certification tracks</h2>
      <div style={{display:'grid', gridTemplateColumns:'repeat(3,1fr)', gap:16}}>
        {[
          { t:'IT Service Management', d:'ITIL-aligned 5-course path', n:5, p:68 },
          { t:'Project Management', d:'PMP-aligned 4-course path', n:4, p:74 },
          { t:'Workplace Safety', d:'OSH levels 1–3', n:3, p:81 },
        ].map((tr,i)=>(
          <Card key={i}>
            <div style={{display:'flex', gap:12, alignItems:'center', marginBottom:14}}>
              <div style={{width:44, height:44, borderRadius:8, background:'#CCFBF1', color:'#0F766E', display:'flex', alignItems:'center', justifyContent:'center'}}>{I.award}</div>
              <div>
                <div style={{font:'600 16px/22px Inter, sans-serif', color:'#1A1A2E'}}>{tr.t}</div>
                <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280'}}>{tr.d}</div>
              </div>
            </div>
            <div style={{display:'flex', justifyContent:'space-between', font:'500 12px/16px Inter, sans-serif', color:'#6B7280', marginBottom:6}}>
              <span>{tr.n} courses</span><span style={{fontVariantNumeric:'tabular-nums'}}>{tr.p}% completion</span>
            </div>
            <div style={{height:6, background:'#F3F4F6', borderRadius:99}}><div style={{width:`${tr.p}%`, height:'100%', background:'#0D9488', borderRadius:99}}/></div>
          </Card>
        ))}
      </div>
    </section>

    <footer style={{background:'#1A1A2E', color:'#fff', padding:'40px 32px'}}>
      <div style={{maxWidth:1200, margin:'0 auto', display:'grid', gridTemplateColumns:'2fr 1fr 1fr 1fr', gap:32}}>
        <div>
          <img src="../../assets/logo.svg" alt="Taalam" style={{height:28, filter:'brightness(0) invert(1)'}}/>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'rgba(255,255,255,0.7)', marginTop:14, maxWidth:320}}>Bahrain's professional training and certification management platform.</div>
        </div>
        {[
          {h:'Platform', l:['Browse courses','Verify certificate','How it works','Pricing']},
          {h:'Company', l:['About','Instructors','Contact','Careers']},
          {h:'Legal', l:['Terms of service','Privacy policy','Refund policy']},
        ].map(c => (
          <div key={c.h}>
            <div style={{font:'600 12px/16px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', color:'rgba(255,255,255,0.6)', marginBottom:12}}>{c.h}</div>
            <div style={{display:'flex', flexDirection:'column', gap:8}}>
              {c.l.map(i => <a key={i} href="#" style={{font:'400 14px/22px Inter, sans-serif', color:'rgba(255,255,255,0.85)', textDecoration:'none'}}>{i}</a>)}
            </div>
          </div>
        ))}
      </div>
      <div style={{maxWidth:1200, margin:'32px auto 0', paddingTop:24, borderTop:'1px solid rgba(255,255,255,0.1)', display:'flex', justifyContent:'space-between', font:'400 12px/16px Inter, sans-serif', color:'rgba(255,255,255,0.6)'}}>
        <span>© 2026 Taalam Training Institute · Manama, Bahrain</span>
        <span>Made with care in the Kingdom of Bahrain</span>
      </div>
    </footer>
  </div>
);

// =================== Public Cert Lookup ===================
const CertLookup = ({ onBack }) => {
  const [shown, setShown] = useState(null);
  return (
    <div style={{background:'#F8F9FA', minHeight:'100vh', padding:'48px 24px'}} data-screen-label="Public · Verify certificate">
      <div style={{maxWidth:680, margin:'0 auto'}}>
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:32}}>
          <img src="../../assets/logo.svg" alt="Taalam" style={{height:28}}/>
          <a href="#" onClick={e=>{e.preventDefault(); onBack();}} style={{font:'500 13px/20px Inter, sans-serif', color:'#6B7280', textDecoration:'none'}}>← Back to home</a>
        </div>
        <Card>
          <div style={{textAlign:'center', marginBottom:24}}>
            <div style={{width:56, height:56, borderRadius:12, background:'#CCFBF1', color:'#0F766E', display:'inline-flex', alignItems:'center', justifyContent:'center', marginBottom:14}}>{I.award}</div>
            <div style={{font:'700 24px/32px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Verify certification</div>
            <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:6}}>Enter trainee details to verify certification status.</div>
          </div>
          <div style={{display:'grid', gridTemplateColumns:'1fr 1fr', gap:14}}>
            <div>
              <label style={{display:'block', font:'600 12px/16px Inter, sans-serif', color:'#1A1A2E', marginBottom:6}}>Trainee ID</label>
              <input defaultValue="TR-00284" style={{width:'100%', padding:'10px 12px', border:'1px solid #E5E7EB', borderRadius:4, font:'400 14px/22px Inter, sans-serif', background:'#fff'}}/>
            </div>
            <div>
              <label style={{display:'block', font:'600 12px/16px Inter, sans-serif', color:'#1A1A2E', marginBottom:6}}>Certificate reference</label>
              <input defaultValue="CERT-2026-0142" style={{width:'100%', padding:'10px 12px', border:'1px solid #E5E7EB', borderRadius:4, font:'400 14px/22px Inter, sans-serif', background:'#fff'}}/>
            </div>
          </div>
          <div style={{display:'flex', gap:10, marginTop:18}}>
            <Btn full onClick={()=>setShown('valid')}>Verify</Btn>
            <Btn variant="ghost" onClick={()=>setShown('invalid')}>Try invalid</Btn>
          </div>
        </Card>

        {shown==='valid' && (
          <Card style={{marginTop:16}}>
            <div style={{display:'flex', alignItems:'center', gap:10, padding:'12px 14px', background:'#D1FAE5', borderRadius:8, color:'#065F46', font:'600 14px/20px Inter, sans-serif', marginBottom:18}}>
              <span style={{display:'inline-flex'}}>{I.check}</span> This certificate is valid.
            </div>
            <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-start', marginBottom:18}}>
              <div>
                <div style={{font:'600 11px/14px Inter, sans-serif', color:'#6B7280', textTransform:'uppercase', letterSpacing:'0.06em'}}>Trainee</div>
                <div style={{font:'700 22px/28px Inter, sans-serif', color:'#1A1A2E', marginTop:4}}>Layla Mansour</div>
                <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:2}}>Trainee ID · TR-00284</div>
              </div>
              <Badge status="Awarded"/>
            </div>
            <div style={{padding:'18px 0', borderTop:'1px solid #EEF0F2', borderBottom:'1px solid #EEF0F2', display:'grid', gridTemplateColumns:'repeat(2,1fr)', gap:14}}>
              <Row label="Certification" value="Workplace Safety, Level 1"/>
              <Row label="Certificate number" value="CERT-2026-0142"/>
              <Row label="Issue date" value="14 Mar 2026"/>
              <Row label="Expires" value="14 Mar 2028"/>
            </div>
            <div style={{marginTop:18}}>
              <div style={{font:'600 12px/16px Inter, sans-serif', color:'#6B7280', textTransform:'uppercase', letterSpacing:'0.04em', marginBottom:10}}>Completed courses</div>
              <div style={{display:'flex', flexDirection:'column', gap:6}}>
                {[{c:'OSH-100',t:'Workplace Safety Fundamentals'},{c:'OSH-110',t:'Hazard Identification'},{c:'OSH-120',t:'Emergency Response'}].map((co,i)=>(
                  <div key={i} style={{display:'flex', alignItems:'center', gap:10, padding:'8px 12px', background:'#F8F9FA', borderRadius:8}}>
                    <span style={{color:'#0F766E'}}>{I.check}</span>
                    <span style={{font:'500 13px/20px Inter, sans-serif', color:'#0F766E', fontFamily:'SFMono-Regular, ui-monospace, monospace'}}>{co.c}</span>
                    <span style={{font:'400 13px/20px Inter, sans-serif', color:'#1A1A2E'}}>{co.t}</span>
                  </div>
                ))}
              </div>
            </div>
          </Card>
        )}
        {shown==='invalid' && (
          <Card style={{marginTop:16}}>
            <div style={{display:'flex', alignItems:'center', gap:10, padding:'12px 14px', background:'#FEE2E2', borderRadius:8, color:'#991B1B', font:'600 14px/20px Inter, sans-serif'}}>
              <span style={{display:'inline-flex'}}>{I.x}</span> Certificate not found. Check the trainee ID and reference number.
            </div>
          </Card>
        )}
      </div>
    </div>
  );
};
const Row = ({ label, value }) => (
  <div>
    <div style={{font:'600 11px/14px Inter, sans-serif', color:'#6B7280', textTransform:'uppercase', letterSpacing:'0.04em', marginBottom:4}}>{label}</div>
    <div style={{font:'500 14px/22px Inter, sans-serif', color:'#1A1A2E'}}>{value}</div>
  </div>
);

// =================== Access Denied ===================
const AccessDenied = ({ onDashboard, onSignOut }) => (
  <div style={{background:'#F8F9FA', minHeight:'100vh', display:'flex', alignItems:'center', justifyContent:'center', padding:24}} data-screen-label="Public · Access denied">
    <Card style={{maxWidth:440, width:'100%', textAlign:'center', padding:40}}>
      <div style={{width:72, height:72, borderRadius:999, background:'#CCFBF1', color:'#0F766E', display:'inline-flex', alignItems:'center', justifyContent:'center', marginBottom:20}}>
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round"><path d="M12 2 5 5v6c0 4.5 3 8.5 7 11 4-2.5 7-6.5 7-11V5z"/><path d="M9 12l2 2 4-4"/></svg>
      </div>
      <div style={{font:'700 24px/32px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Access denied</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:8, marginBottom:24}}>You don't have permission to view this page. Contact your training coordinator if you believe this is an error.</div>
      <div style={{display:'flex', gap:10, justifyContent:'center'}}>
        <Btn onClick={onDashboard}>Go to dashboard</Btn>
        <Btn variant="secondary" onClick={onSignOut}>Sign out</Btn>
      </div>
    </Card>
  </div>
);

Object.assign(window, { Landing, CertLookup, AccessDenied });
