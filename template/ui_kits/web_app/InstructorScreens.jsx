/* global React, I, Btn, Badge, Card, Stat, Table, Field, Input, Select, Avatar */
const { useState } = React;

const InstructorDashboard = ({ goto }) => (
  <>
    <div style={{marginBottom:24}}>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280'}}>Welcome back, Abdulla.</div>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Your teaching today</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:16, marginBottom:24}}>
      <Stat label="Sessions this week" value="5" hint="2 today" icon={I.cal}/>
      <Stat label="Trainees" value="84" hint="across 5 sessions" icon={I.users}/>
      <Stat label="Pending assessments" value="12" hint="from 3 sessions" icon={I.clip}/>
      <Stat label="Avg pass rate" value="92%" hint="last 30 days" trend="up" icon={I.bar}/>
    </div>
    <Card title="Today's sessions" action={<Btn variant="ghost" size="sm" onClick={()=>goto('sessions')}>All sessions {I.chev}</Btn>}>
      <Table
        columns={[
          { header:'Time', key:'t', num:true },
          { header:'Course', key:'course' },
          { header:'Classroom', key:'room' },
          { header:'Enrolled', render:r => <span style={{fontVariantNumeric:'tabular-nums'}}>{r.e} / {r.c}</span> },
          { header:'Status', render:r => <Badge status={r.s}/> },
          { header:'', render:()=> <Btn size="sm" variant="secondary" onClick={()=>goto('session')}>Open</Btn>, align:'right' },
        ]}
        rows={[
          { t:'09:00 – 12:00', course:'Project Management Fundamentals', room:'Room 204', e:14, c:20, s:'In progress' },
          { t:'14:00 – 17:00', course:'Anti-Money Laundering Essentials', room:'Room 110', e:21, c:30, s:'Scheduled' },
        ]}
      />
    </Card>
  </>
);

const MySessions = ({ goto }) => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My sessions</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Sessions assigned to you across the next 60 days.</div>
    </div>
    <Card padded={false}>
      <Table
        columns={[
          { header:'Date', key:'d', num:true },
          { header:'Time', key:'t', num:true },
          { header:'Course', key:'course' },
          { header:'Classroom', key:'room' },
          { header:'Enrolled', render:r => <span style={{fontVariantNumeric:'tabular-nums'}}>{r.e} / {r.c}</span> },
          { header:'Status', render:r => <Badge status={r.s}/> },
          { header:'', render:()=> <Btn size="sm" variant="secondary" onClick={()=>goto('session')}>Open</Btn>, align:'right' },
        ]}
        rows={[
          { d:'07 May 2026', t:'09:00 – 12:00', course:'Project Management Fundamentals', room:'Room 204', e:14, c:20, s:'In progress' },
          { d:'07 May 2026', t:'14:00 – 17:00', course:'Anti-Money Laundering', room:'Room 110', e:21, c:30, s:'Scheduled' },
          { d:'12 May 2026', t:'14:00 – 17:00', course:'Project Management Fundamentals', room:'Room 204', e:14, c:20, s:'Scheduled' },
          { d:'18 Jun 2026', t:'09:00 – 12:00', course:'Anti-Money Laundering', room:'Room 110', e:0, c:30, s:'Scheduled' },
          { d:'02 Apr 2026', t:'14:00 – 17:00', course:'Project Management Fundamentals', room:'Room 204', e:18, c:20, s:'Completed' },
        ]}
      />
    </Card>
  </>
);

const SessionDetail = ({ goto }) => (
  <>
    <button onClick={()=>goto('sessions')} style={{display:'inline-flex', alignItems:'center', gap:6, background:'transparent', border:0, color:'#6B7280', font:'500 13px/20px Inter, sans-serif', cursor:'pointer', marginBottom:14}}>
      <span style={{transform:'rotate(180deg)', display:'inline-flex'}}>{I.chev}</span> Back to sessions
    </button>
    <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-start', marginBottom:20}}>
      <div>
        <div style={{font:'500 12px/16px Inter, sans-serif', color:'#0D9488', letterSpacing:'0.04em'}}>SES-2026-0142 · PMP-100</div>
        <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', marginTop:4}}>Project Management Fundamentals</div>
        <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:6, fontVariantNumeric:'tabular-nums'}}>12 May 2026 · 14:00 – 17:00 · Room 204</div>
      </div>
      <div style={{display:'flex', gap:10}}>
        <Btn variant="secondary">Mark attendance</Btn>
        <Btn icon={I.clip} onClick={()=>goto('assess')}>Record assessment</Btn>
      </div>
    </div>
    <Card padded={false} title={<></>}>
      <div style={{padding:'14px 20px', borderBottom:'1px solid #EEF0F2', display:'flex', justifyContent:'space-between', alignItems:'center'}}>
        <div style={{font:'600 16px/24px Inter, sans-serif'}}>Trainees · 14 enrolled</div>
        <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280'}}>Showing all</div>
      </div>
      <Table
        columns={[
          { header:'Trainee', render:r => <div style={{display:'flex', alignItems:'center', gap:10}}><Avatar name={r.name} size={28}/><div><div style={{fontWeight:500}}>{r.name}</div><div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280'}}>{r.email}</div></div></div> },
          { header:'Attendance', render:r => <Badge status={r.att}/> },
          { header:'Assessment', render:r => r.result==='—' ? <span style={{color:'#9CA3AF'}}>—</span> : <Badge status={r.result}/> },
          { header:'', render:()=> <Btn size="sm" variant="ghost" onClick={()=>goto('assess')}>Record</Btn>, align:'right' },
        ]}
        rows={[
          { name:'Layla Mansour', email:'layla@taalam.bh', att:'Active', result:'Pass' },
          { name:'Omar Saeed', email:'omar@example.bh', att:'Active', result:'—' },
          { name:'Yousef Rashid', email:'yousef@example.bh', att:'Active', result:'Pending' },
          { name:'Reem Hassan', email:'reem@example.bh', att:'Active', result:'Pass' },
          { name:'Tariq Hamad', email:'tariq@example.bh', att:'Inactive', result:'Fail' },
        ]}
      />
    </Card>
  </>
);

const RecordAssessment = ({ goto }) => {
  const [result, setResult] = useState('Pass');
  return (
    <>
      <button onClick={()=>goto('session')} style={{display:'inline-flex', alignItems:'center', gap:6, background:'transparent', border:0, color:'#6B7280', font:'500 13px/20px Inter, sans-serif', cursor:'pointer', marginBottom:14}}>
        <span style={{transform:'rotate(180deg)', display:'inline-flex'}}>{I.chev}</span> Back to session
      </button>
      <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:20}}>
        <Card title="Record assessment">
          <div style={{display:'flex', flexDirection:'column', gap:16}}>
            <Field label="Trainee">
              <Select><option>Omar Saeed · ENR-2026-0341</option></Select>
            </Field>
            <Field label="Assessment date"><Input defaultValue="07/05/2026"/></Field>
            <div>
              <label style={{display:'block', font:'600 12px/16px Inter, sans-serif', color:'#1A1A2E', marginBottom:8}}>Result</label>
              <div style={{display:'flex', gap:8}}>
                {['Pass','Fail','Pending'].map(r => (
                  <button key={r} onClick={()=>setResult(r)} style={{
                    flex:1, padding:'12px 14px', borderRadius:8,
                    border: result===r ? '1px solid #0D9488' : '1px solid #E5E7EB',
                    background: result===r ? 'rgba(13,148,136,0.08)' : '#fff',
                    color: result===r ? '#0F766E' : '#1A1A2E',
                    font: result===r ? '600 14px/22px Inter, sans-serif' : '500 14px/22px Inter, sans-serif',
                    cursor:'pointer'
                  }}>{r}</button>
                ))}
              </div>
            </div>
            <Field label="Remarks" hint="Visible to the trainee and the coordinator.">
              <textarea defaultValue="Strong grasp of project lifecycle and risk management. Recommend Anti-Money Laundering as next course." style={{
                width:'100%', minHeight:120, padding:'10px 12px', border:'1px solid #E5E7EB',
                borderRadius:4, font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E', resize:'vertical', background:'#fff'
              }}/>
            </Field>
            <div style={{display:'flex', gap:10, justifyContent:'flex-end'}}>
              <Btn variant="ghost" onClick={()=>goto('session')}>Cancel</Btn>
              <Btn>Save assessment</Btn>
            </div>
          </div>
        </Card>
        <div style={{display:'flex', flexDirection:'column', gap:16}}>
          <Card title="Enrollment summary">
            <div style={{display:'flex', flexDirection:'column', gap:10, font:'400 13px/20px Inter, sans-serif'}}>
              <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Course</span><span style={{color:'#1A1A2E', fontWeight:500}}>PMP-100</span></div>
              <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Session</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>12 May 2026</span></div>
              <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Attendance</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>22 / 24 hrs</span></div>
              <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Status</span><Badge status="Enrolled"/></div>
            </div>
          </Card>
          <Card title="Counts toward">
            <div style={{display:'flex', alignItems:'center', gap:10, font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E'}}>
              <span style={{color:'#0D9488'}}>{I.award}</span>Project Management Track
            </div>
            <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:6}}>A pass here completes the track for this trainee.</div>
          </Card>
        </div>
      </div>
    </>
  );
};

Object.assign(window, { InstructorDashboard, MySessions, SessionDetail, RecordAssessment });
