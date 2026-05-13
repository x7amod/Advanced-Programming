/* global React, I, Btn, Badge, Card, Stat, Table, Avatar */
const { useState } = React;

const COURSES = [
  { id:1, code:'PMP-100', title:'Project Management Fundamentals', area:'Business', hrs:24, fee:120, capacity:20, enrolled:14, instructor:'Fatima Khalifa', next:'12 May 2026', status:'Scheduled' },
  { id:2, code:'OSH-200', title:'Occupational Safety & Health, Level 2', area:'Safety', hrs:16, fee:85, capacity:25, enrolled:25, instructor:'Khalid Nasser', next:'20 May 2026', status:'In progress' },
  { id:3, code:'ITIL-300', title:'ITIL 4 Foundation', area:'IT', hrs:32, fee:240, capacity:18, enrolled:12, instructor:'Layla Mansour', next:'04 Jun 2026', status:'Scheduled' },
  { id:4, code:'AML-110', title:'Anti-Money Laundering Essentials', area:'Finance', hrs:12, fee:95, capacity:30, enrolled:21, instructor:'Fatima Khalifa', next:'18 Jun 2026', status:'Scheduled' },
];

const TraineeDashboard = ({ goto }) => (
  <>
    <div style={{marginBottom:24}}>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280'}}>Welcome back, Abdulla.</div>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Your training at a glance</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:16, marginBottom:24}}>
      <Stat label="Active enrollments" value="3" hint="2 sessions this month" icon={I.cal}/>
      <Stat label="Hours completed" value="48" hint="of 96 required" icon={I.clock}/>
      <Stat label="Certifications" value="2" hint="1 in progress" icon={I.award}/>
      <Stat label="Outstanding fees" value="BHD 85" hint="Due 20 May" icon={I.card}/>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16}}>
      <Card title="Upcoming sessions" action={<Btn variant="ghost" size="sm" onClick={()=>goto('enrollments')}>View all {I.chev}</Btn>}>
        <Table
          columns={[
            { header:'Course', key:'title' },
            { header:'Date', key:'date', num:true },
            { header:'Status', render: r => <Badge status={r.s}/> },
          ]}
          rows={[
            { title:'Project Management Fundamentals', date:'12 May 2026 · 14:00', s:'Enrolled' },
            { title:'Occupational Safety, Level 2', date:'20 May 2026 · 09:00', s:'Waitlisted' },
            { title:'ITIL 4 Foundation', date:'04 Jun 2026 · 10:00', s:'Enrolled' },
          ]}
        />
      </Card>
      <Card title="Certification progress">
        <div style={{display:'flex', flexDirection:'column', gap:14}}>
          <div>
            <div style={{display:'flex', justifyContent:'space-between', marginBottom:6}}>
              <div style={{font:'500 13px/20px Inter, sans-serif', color:'#1A1A2E'}}>IT Service Management</div>
              <div style={{font:'500 13px/20px Inter, sans-serif', color:'#6B7280', fontVariantNumeric:'tabular-nums'}}>3 / 5</div>
            </div>
            <div style={{height:6, background:'#F3F4F6', borderRadius:99}}>
              <div style={{width:'60%', height:'100%', background:'#0D9488', borderRadius:99}}/>
            </div>
          </div>
          <div>
            <div style={{display:'flex', justifyContent:'space-between', marginBottom:6}}>
              <div style={{font:'500 13px/20px Inter, sans-serif', color:'#1A1A2E'}}>Project Management Track</div>
              <div style={{font:'500 13px/20px Inter, sans-serif', color:'#6B7280', fontVariantNumeric:'tabular-nums'}}>1 / 4</div>
            </div>
            <div style={{height:6, background:'#F3F4F6', borderRadius:99}}>
              <div style={{width:'25%', height:'100%', background:'#0D9488', borderRadius:99}}/>
            </div>
          </div>
        </div>
      </Card>
    </div>
  </>
);

const BrowseCourses = ({ goto, setCourse }) => {
  const [q, setQ] = useState('');
  const filtered = COURSES.filter(c => c.title.toLowerCase().includes(q.toLowerCase()));
  return (
    <>
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20}}>
        <div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Browse courses</div>
          <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>{COURSES.length} courses available across 8 subject areas.</div>
        </div>
      </div>
      <div style={{display:'flex', gap:12, marginBottom:20}}>
        <div style={{position:'relative', flex:1}}>
          <span style={{position:'absolute', left:12, top:'50%', transform:'translateY(-50%)', color:'#9CA3AF'}}>{I.search}</span>
          <input value={q} onChange={e=>setQ(e.target.value)} placeholder="Search by title or course code…" style={{width:'100%', padding:'10px 12px 10px 38px', borderRadius:8, border:'1px solid #E5E7EB', font:'400 14px/22px Inter, sans-serif', background:'#fff'}}/>
        </div>
        <select style={{padding:'10px 32px 10px 12px', borderRadius:8, border:'1px solid #E5E7EB', background:'#fff', font:'400 14px/22px Inter, sans-serif'}}><option>All subject areas</option></select>
        <select style={{padding:'10px 32px 10px 12px', borderRadius:8, border:'1px solid #E5E7EB', background:'#fff', font:'400 14px/22px Inter, sans-serif'}}><option>Any duration</option></select>
      </div>
      <div style={{display:'grid', gridTemplateColumns:'repeat(2, 1fr)', gap:16}}>
        {filtered.map(c => (
          <Card key={c.id}>
            <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-start', marginBottom:8}}>
              <div>
                <div style={{font:'500 12px/16px Inter, sans-serif', color:'#0D9488', letterSpacing:'0.04em'}}>{c.code} · {c.area}</div>
                <div style={{font:'600 18px/26px Inter, sans-serif', color:'#1A1A2E', marginTop:4}}>{c.title}</div>
              </div>
              <Badge status={c.status}/>
            </div>
            <div style={{display:'flex', gap:20, font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:10}}>
              <span><strong style={{color:'#1A1A2E', fontWeight:500}}>{c.hrs} hrs</strong> · duration</span>
              <span><strong style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{c.enrolled}/{c.capacity}</strong> · enrolled</span>
              <span style={{fontVariantNumeric:'tabular-nums'}}><strong style={{color:'#1A1A2E', fontWeight:500}}>BHD {c.fee.toFixed(3)}</strong></span>
            </div>
            <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginTop:14, paddingTop:14, borderTop:'1px solid #EEF0F2'}}>
              <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280'}}>Next session: <strong style={{color:'#1A1A2E', fontWeight:500}}>{c.next}</strong></div>
              <Btn size="sm" onClick={()=>{ setCourse(c); goto('course'); }}>View details</Btn>
            </div>
          </Card>
        ))}
      </div>
    </>
  );
};

const CourseDetail = ({ course, goto }) => (
  <>
    <button onClick={()=>goto('browse')} style={{display:'inline-flex', alignItems:'center', gap:6, background:'transparent', border:0, color:'#6B7280', font:'500 13px/20px Inter, sans-serif', cursor:'pointer', marginBottom:14}}>
      <span style={{transform:'rotate(180deg)', display:'inline-flex'}}>{I.chev}</span> Back to courses
    </button>
    <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:20}}>
      <div style={{display:'flex', flexDirection:'column', gap:16}}>
        <Card>
          <div style={{font:'500 12px/16px Inter, sans-serif', color:'#0D9488', letterSpacing:'0.04em'}}>{course.code} · {course.area}</div>
          <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', marginTop:6, marginBottom:10}}>{course.title}</div>
          <div style={{font:'400 15px/24px Inter, sans-serif', color:'#6B7280'}}>
            A practical introduction to managing projects from initiation to closure. Covers scope, scheduling, budgeting, risk, stakeholder communication, and the project life cycle. Suitable for working professionals new to project management.
          </div>
        </Card>
        <Card title="What you'll learn">
          <ul style={{margin:0, padding:0, listStyle:'none', display:'flex', flexDirection:'column', gap:8}}>
            {['Define project scope and deliverables','Build realistic schedules and budgets','Identify and mitigate project risk','Manage stakeholder communications','Apply Agile and Waterfall methods'].map((t,i)=>(
              <li key={i} style={{display:'flex', gap:10, font:'400 14px/22px Inter, sans-serif', color:'#1A1A2E'}}>
                <span style={{color:'#0D9488', flex:'none'}}>{I.check}</span>{t}
              </li>
            ))}
          </ul>
        </Card>
        <Card title="Available sessions">
          <Table
            columns={[
              { header:'Date', key:'d', num:true },
              { header:'Time', key:'t', num:true },
              { header:'Instructor', key:'i' },
              { header:'Seats', render:r => (
                <div style={{display:'flex', flexDirection:'column', gap:2}}>
                  <span style={{fontVariantNumeric:'tabular-nums'}}>{r.left} of {r.cap}</span>
                  {r.left>0 && (
                    <span style={{display:'inline-flex', alignItems:'center', gap:6, font:'500 11px/14px Inter, sans-serif', color:r.left<5?'#92400E':'#0F766E'}}>
                      <span style={{width:6, height:6, borderRadius:999, background:r.left<5?'#F59E0B':'#10B981', boxShadow:`0 0 0 3px ${r.left<5?'rgba(245,158,11,0.18)':'rgba(16,185,129,0.18)'}`, animation:'pulse 1.6s ease-in-out infinite'}}/>
                      {r.left} spots remaining · live
                    </span>
                  )}
                </div>
              ), align:'right' },
              { header:'', render: r => r.left>0 ? <Btn size="sm">Enroll</Btn> : <Btn size="sm" variant="secondary">Join waitlist</Btn>, align:'right' },
            ]}
            rows={[
              { d:'12 May 2026', t:'14:00 – 17:00', i:'Fatima Khalifa', cap:20, left:6 },
              { d:'26 May 2026', t:'09:00 – 12:00', i:'Fatima Khalifa', cap:20, left:14 },
              { d:'09 Jun 2026', t:'14:00 – 17:00', i:'Layla Mansour', cap:20, left:0 },
            ]}
          />
          <style>{`@keyframes pulse { 0%,100%{opacity:1} 50%{opacity:.45} }`}</style>
        </Card>
      </div>
      <div style={{display:'flex', flexDirection:'column', gap:16}}>
        <Card>
          <div style={{font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.06em', color:'#6B7280'}}>Enrollment fee</div>
          <div style={{font:'700 32px/40px Inter, sans-serif', color:'#1A1A2E', fontVariantNumeric:'tabular-nums', letterSpacing:'-0.01em', margin:'4px 0 14px'}}>BHD {course.fee.toFixed(3)}</div>
          <Btn full>Enroll now</Btn>
          <div style={{borderTop:'1px solid #EEF0F2', marginTop:16, paddingTop:14, display:'flex', flexDirection:'column', gap:10, font:'400 13px/20px Inter, sans-serif'}}>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Duration</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{course.hrs} hours</span></div>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Capacity</span><span style={{color:'#1A1A2E', fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{course.capacity} seats</span></div>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Subject area</span><span style={{color:'#1A1A2E', fontWeight:500}}>{course.area}</span></div>
            <div style={{display:'flex', justifyContent:'space-between'}}><span style={{color:'#6B7280'}}>Prerequisites</span><span style={{color:'#1A1A2E', fontWeight:500}}>None</span></div>
          </div>
        </Card>
        <Card title="Counts toward">
          <div style={{display:'flex', flexDirection:'column', gap:10, font:'400 14px/22px Inter, sans-serif'}}>
            <a href="#" style={{display:'flex', alignItems:'center', gap:10, color:'#1A1A2E', textDecoration:'none'}}>
              <span style={{color:'#0D9488'}}>{I.award}</span>Project Management Track
            </a>
            <a href="#" style={{display:'flex', alignItems:'center', gap:10, color:'#1A1A2E', textDecoration:'none'}}>
              <span style={{color:'#0D9488'}}>{I.award}</span>Business Operations, Level 1
            </a>
          </div>
        </Card>
      </div>
    </div>
  </>
);

const MyEnrollments = () => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My enrollments</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Active and past course enrollments.</div>
    </div>
    <Card padded={false}>
      <Table
        columns={[
          { header:'Course', render:r => <div><div style={{fontWeight:500}}>{r.title}</div><div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280'}}>{r.code}</div></div> },
          { header:'Session', key:'session', num:true },
          { header:'Instructor', key:'instructor' },
          { header:'Status', render:r => <Badge status={r.status}/> },
          { header:'Fee', render:r => <span>BHD {r.fee.toFixed(3)}</span>, num:true, align:'right' },
        ]}
        rows={[
          { code:'PMP-100', title:'Project Management Fundamentals', session:'12 May 2026', instructor:'Fatima Khalifa', status:'Enrolled', fee:120 },
          { code:'OSH-200', title:'Occupational Safety, Level 2', session:'20 May 2026', instructor:'Khalid Nasser', status:'Waitlisted', fee:85 },
          { code:'ITIL-300', title:'ITIL 4 Foundation', session:'04 Jun 2026', instructor:'Layla Mansour', status:'Enrolled', fee:240 },
          { code:'AML-110', title:'Anti-Money Laundering Essentials', session:'14 Mar 2026', instructor:'Fatima Khalifa', status:'Completed', fee:95 },
          { code:'EXC-101', title:'Excel for Analysts', session:'02 Feb 2026', instructor:'Layla Mansour', status:'Dropped', fee:60 },
        ]}
      />
    </Card>
  </>
);

const MyCertifications = () => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My certifications</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Awarded credentials and tracks in progress.</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(2, 1fr)', gap:16}}>
      {[
        { name:'Occupational Safety, Level 1', status:'Awarded', date:'14 Mar 2026', valid:'14 Mar 2028' },
        { name:'IT Service Management', status:'In progress', date:null, prog:60, courses:'3 of 5 required' },
        { name:'Project Management Track', status:'In progress', date:null, prog:25, courses:'1 of 4 required' },
        { name:'Anti-Money Laundering', status:'Expired', date:'14 Jan 2024', valid:'14 Jan 2026' },
      ].map((c,i) => (
        <Card key={i}>
          <div style={{display:'flex', justifyContent:'space-between', alignItems:'flex-start', marginBottom:12}}>
            <div style={{display:'flex', gap:12, alignItems:'center'}}>
              <div style={{width:44, height:44, borderRadius:8, background:'#CCFBF1', color:'#0F766E', display:'flex', alignItems:'center', justifyContent:'center'}}>{I.award}</div>
              <div>
                <div style={{font:'600 16px/22px Inter, sans-serif', color:'#1A1A2E'}}>{c.name}</div>
                {c.date && <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:2, fontVariantNumeric:'tabular-nums'}}>Awarded {c.date} · valid until {c.valid}</div>}
                {c.prog!=null && <div style={{font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:2}}>{c.courses}</div>}
              </div>
            </div>
            <Badge status={c.status}/>
          </div>
          {c.prog!=null ? (
            <div style={{height:6, background:'#F3F4F6', borderRadius:99}}>
              <div style={{width:`${c.prog}%`, height:'100%', background:'#0D9488', borderRadius:99}}/>
            </div>
          ) : c.status==='Awarded' ? (
            <Btn size="sm" variant="secondary" icon={I.download}>Download certificate</Btn>
          ) : (
            <Btn size="sm" variant="secondary">Renew certification</Btn>
          )}
        </Card>
      ))}
    </div>
  </>
);

const MyPayments = () => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>My payments</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Invoices, transactions, and outstanding balances.</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(3, 1fr)', gap:16, marginBottom:20}}>
      <Stat label="Outstanding" value="BHD 85.000" hint="Due 20 May 2026" icon={I.card}/>
      <Stat label="Paid this year" value="BHD 460.000" hint="4 invoices" icon={I.check}/>
      <Stat label="Refunded" value="BHD 60.000" hint="1 transaction" icon={I.download}/>
    </div>
    <Card padded={false} title={<></>}>
      <Table
        columns={[
          { header:'Invoice', render:r => <span style={{fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{r.id}</span> },
          { header:'Course', key:'course' },
          { header:'Issued', key:'issued', num:true },
          { header:'Due', key:'due', num:true },
          { header:'Status', render:r => <Badge status={r.status}/> },
          { header:'Amount', render:r => <span>BHD {r.amt.toFixed(3)}</span>, num:true, align:'right' },
        ]}
        rows={[
          { id:'INV-2026-0142', course:'Occupational Safety, Level 2', issued:'06 May 2026', due:'20 May 2026', status:'Pending', amt:85 },
          { id:'INV-2026-0119', course:'Project Management Fundamentals', issued:'28 Apr 2026', due:'12 May 2026', status:'Paid', amt:120 },
          { id:'INV-2026-0098', course:'ITIL 4 Foundation', issued:'15 Apr 2026', due:'04 Jun 2026', status:'Partially paid', amt:240 },
          { id:'INV-2026-0061', course:'Anti-Money Laundering Essentials', issued:'02 Mar 2026', due:'14 Mar 2026', status:'Paid', amt:95 },
        ]}
      />
    </Card>
  </>
);

Object.assign(window, { TraineeDashboard, BrowseCourses, CourseDetail, MyEnrollments, MyCertifications, MyPayments });
