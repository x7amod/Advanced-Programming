/* global React, I, Btn, Badge, Card, Stat, Table, Avatar */
const { useState } = React;

const CoordinatorDashboard = ({ goto }) => (
  <>
    <div style={{marginBottom:24}}>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280'}}>Welcome back, Abdulla.</div>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Institute overview</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:16, marginBottom:24}}>
      <Stat label="Active trainees" value="284" hint="+18 this month" trend="up" icon={I.users}/>
      <Stat label="Sessions this week" value="42" hint="38 confirmed" icon={I.cal}/>
      <Stat label="Revenue MTD" value="BHD 24,820" hint="vs BHD 21,400 last" trend="up" icon={I.card}/>
      <Stat label="Outstanding fees" value="BHD 6,420" hint="14 invoices overdue" trend="down" icon={I.bar}/>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16}}>
      <Card title="Recent enrollments" action={<Btn variant="ghost" size="sm" onClick={()=>goto('enrollments')}>View all {I.chev}</Btn>}>
        <Table
          columns={[
            { header:'Trainee', render:r => <div style={{display:'flex', alignItems:'center', gap:10}}><Avatar name={r.name} size={28}/><span style={{fontWeight:500}}>{r.name}</span></div> },
            { header:'Course', key:'course' },
            { header:'Session', key:'date', num:true },
            { header:'Status', render:r => <Badge status={r.s}/> },
          ]}
          rows={[
            { name:'Layla Mansour', course:'PMP-100', date:'12 May 2026', s:'Enrolled' },
            { name:'Omar Saeed', course:'OSH-200', date:'20 May 2026', s:'Waitlisted' },
            { name:'Reem Hassan', course:'ITIL-300', date:'04 Jun 2026', s:'Enrolled' },
            { name:'Tariq Hamad', course:'AML-110', date:'18 Jun 2026', s:'Pending' },
          ]}
        />
      </Card>
      <Card title="Action queue">
        <div style={{display:'flex', flexDirection:'column', gap:10}}>
          {[
            { ic:I.clip, txt:'4 assessments awaiting approval', cta:'Review' },
            { ic:I.users, txt:'12 trainees on waitlist', cta:'Add session' },
            { ic:I.card, txt:'14 invoices overdue', cta:'Send reminder' },
            { ic:I.award, txt:'7 certifications ready to award', cta:'Issue' },
          ].map((a,i)=> (
            <div key={i} style={{display:'flex', alignItems:'center', gap:12, padding:'10px 12px', background:'#F8F9FA', borderRadius:8}}>
              <div style={{width:32, height:32, borderRadius:8, background:'#fff', display:'flex', alignItems:'center', justifyContent:'center', color:'#0D9488', flex:'none'}}>{a.ic}</div>
              <div style={{flex:1, font:'500 13px/18px Inter, sans-serif', color:'#1A1A2E'}}>{a.txt}</div>
              <button style={{background:'transparent', border:0, color:'#0D9488', font:'600 13px/18px Inter, sans-serif', cursor:'pointer'}}>{a.cta}</button>
            </div>
          ))}
        </div>
      </Card>
    </div>
  </>
);

const Users = () => (
  <>
    <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20}}>
      <div>
        <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Users</div>
        <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Trainees, instructors, and coordinators across the institute.</div>
      </div>
      <Btn icon={I.plus}>Add user</Btn>
    </div>
    <div style={{display:'flex', gap:8, marginBottom:16}}>
      {['All · 318','Trainees · 284','Instructors · 28','Coordinators · 6'].map((t,i) => (
        <button key={i} style={{
          padding:'8px 14px', borderRadius:8,
          border: i===0?'1px solid #0D9488':'1px solid #E5E7EB',
          background: i===0?'rgba(13,148,136,0.08)':'#fff',
          color: i===0?'#0F766E':'#6B7280',
          font: i===0?'600 13px/18px Inter, sans-serif':'500 13px/18px Inter, sans-serif',
          cursor:'pointer'
        }}>{t}</button>
      ))}
    </div>
    <Card padded={false}>
      <Table
        columns={[
          { header:'Name', render:r => <div style={{display:'flex', alignItems:'center', gap:10}}><Avatar name={r.name} size={32}/><div><div style={{fontWeight:500}}>{r.name}</div><div style={{font:'400 12px/16px Inter, sans-serif', color:'#6B7280'}}>{r.email}</div></div></div> },
          { header:'Role', render:r => <Badge status={r.role} dot={false}/> },
          { header:'Joined', key:'joined', num:true },
          { header:'Status', render:r => <Badge status={r.s}/> },
          { header:'', render:()=> <div style={{display:'flex', gap:6, justifyContent:'flex-end', color:'#6B7280'}}><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.edit}</button><button style={{background:'transparent', border:0, cursor:'pointer', color:'#6B7280'}}>{I.trash}</button></div>, align:'right' },
        ]}
        rows={[
          { name:'Layla Mansour', email:'layla@taalam.bh', role:'Trainee', joined:'14 Jan 2026', s:'Active' },
          { name:'Fatima Khalifa', email:'fatima@taalam.bh', role:'Instructor', joined:'02 Sep 2024', s:'Active' },
          { name:'Omar Saeed', email:'omar@example.bh', role:'Trainee', joined:'21 Mar 2026', s:'Active' },
          { name:'Yousef Rashid', email:'yousef@taalam.bh', role:'Coordinator', joined:'10 May 2023', s:'Active' },
          { name:'Reem Hassan', email:'reem@example.bh', role:'Trainee', joined:'05 Apr 2026', s:'Active' },
          { name:'Tariq Hamad', email:'tariq@example.bh', role:'Trainee', joined:'30 Apr 2026', s:'Inactive' },
        ]}
      />
    </Card>
  </>
);

const SessionsMgmt = () => (
  <>
    <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20}}>
      <div>
        <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Sessions</div>
        <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Schedule and manage course sessions across classrooms and instructors.</div>
      </div>
      <Btn icon={I.plus}>Schedule session</Btn>
    </div>
    <Card padded={false}>
      <Table
        columns={[
          { header:'ID', render:r => <span style={{fontFamily:'SFMono-Regular, ui-monospace, monospace', font:'500 12px/16px SFMono-Regular, monospace', color:'#6B7280'}}>{r.id}</span> },
          { header:'Course', key:'course' },
          { header:'Instructor', key:'instructor' },
          { header:'Classroom', key:'room' },
          { header:'Date', key:'date', num:true },
          { header:'Enrolled', render:r => <span style={{fontVariantNumeric:'tabular-nums'}}>{r.e} / {r.c}</span> },
          { header:'Status', render:r => <Badge status={r.s}/> },
        ]}
        rows={[
          { id:'SES-0142', course:'PMP-100', instructor:'Fatima Khalifa', room:'Room 204', date:'12 May 2026', e:14, c:20, s:'Scheduled' },
          { id:'SES-0143', course:'OSH-200', instructor:'Khalid Nasser', room:'Lab A', date:'20 May 2026', e:25, c:25, s:'In progress' },
          { id:'SES-0144', course:'ITIL-300', instructor:'Layla Mansour', room:'Room 110', date:'04 Jun 2026', e:12, c:18, s:'Scheduled' },
          { id:'SES-0145', course:'AML-110', instructor:'Fatima Khalifa', room:'Room 110', date:'18 Jun 2026', e:21, c:30, s:'Scheduled' },
          { id:'SES-0140', course:'PMP-100', instructor:'Fatima Khalifa', room:'Room 204', date:'02 Apr 2026', e:18, c:20, s:'Completed' },
          { id:'SES-0138', course:'EXC-101', instructor:'Layla Mansour', room:'Lab B', date:'24 Mar 2026', e:0, c:18, s:'Cancelled' },
        ]}
      />
    </Card>
  </>
);

const PaymentsMgmt = () => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Payments</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Invoices, transactions, and trainee balances.</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(4, 1fr)', gap:16, marginBottom:20}}>
      <Stat label="Collected MTD" value="BHD 24,820" hint="+15.9%" trend="up"/>
      <Stat label="Outstanding" value="BHD 6,420" hint="14 invoices"/>
      <Stat label="Overdue" value="BHD 1,840" hint="5 invoices > 30 days" trend="down"/>
      <Stat label="Refunded MTD" value="BHD 240" hint="2 transactions"/>
    </div>
    <Card padded={false}>
      <Table
        columns={[
          { header:'Invoice', render:r => <span style={{fontWeight:500, fontVariantNumeric:'tabular-nums'}}>{r.id}</span> },
          { header:'Trainee', key:'trainee' },
          { header:'Course', key:'course' },
          { header:'Issued', key:'issued', num:true },
          { header:'Due', key:'due', num:true },
          { header:'Status', render:r => <Badge status={r.s}/> },
          { header:'Amount', render:r => <span>BHD {r.amt.toFixed(3)}</span>, num:true, align:'right' },
        ]}
        rows={[
          { id:'INV-0142', trainee:'Layla Mansour', course:'PMP-100', issued:'06 May 2026', due:'20 May 2026', s:'Pending', amt:120 },
          { id:'INV-0141', trainee:'Omar Saeed', course:'OSH-200', issued:'02 May 2026', due:'16 May 2026', s:'Overdue', amt:85 },
          { id:'INV-0140', trainee:'Reem Hassan', course:'ITIL-300', issued:'28 Apr 2026', due:'12 May 2026', s:'Paid', amt:240 },
          { id:'INV-0139', trainee:'Yousef Rashid', course:'AML-110', issued:'25 Apr 2026', due:'09 May 2026', s:'Partially paid', amt:95 },
          { id:'INV-0138', trainee:'Tariq Hamad', course:'EXC-101', issued:'18 Apr 2026', due:'02 May 2026', s:'Refunded', amt:60 },
        ]}
      />
    </Card>
  </>
);

const Reports = () => (
  <>
    <div style={{marginBottom:20}}>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em'}}>Reports</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginTop:4}}>Insights across enrollments, completion, and revenue.</div>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'2fr 1fr', gap:16, marginBottom:16}}>
      <Card title="Enrollments · last 6 months">
        <div style={{display:'flex', alignItems:'flex-end', gap:14, height:180, padding:'10px 0', borderBottom:'1px solid #EEF0F2'}}>
          {[42,58,49,72,84,96].map((v,i)=>(
            <div key={i} style={{flex:1, display:'flex', flexDirection:'column', alignItems:'center', gap:6}}>
              <div style={{width:'100%', height:`${v*1.5}px`, background:'#0D9488', borderRadius:'6px 6px 0 0'}}/>
              <div style={{font:'500 11px/14px Inter, sans-serif', color:'#6B7280'}}>{['Dec','Jan','Feb','Mar','Apr','May'][i]}</div>
            </div>
          ))}
        </div>
        <div style={{display:'flex', justifyContent:'space-between', marginTop:12, font:'400 13px/20px Inter, sans-serif', color:'#6B7280'}}>
          <span>Total · 401 enrollments</span>
          <span style={{color:'#065F46', fontWeight:500}}>▲ 14% vs prev</span>
        </div>
      </Card>
      <Card title="Top courses">
        <div style={{display:'flex', flexDirection:'column', gap:12}}>
          {[
            { c:'Project Management Fundamentals', n:84 },
            { c:'Occupational Safety, Level 2', n:72 },
            { c:'ITIL 4 Foundation', n:48 },
            { c:'Anti-Money Laundering', n:36 },
          ].map((r,i) => (
            <div key={i}>
              <div style={{display:'flex', justifyContent:'space-between', marginBottom:4, font:'500 13px/18px Inter, sans-serif', color:'#1A1A2E'}}>
                <span>{r.c}</span><span style={{fontVariantNumeric:'tabular-nums', color:'#6B7280'}}>{r.n}</span>
              </div>
              <div style={{height:6, background:'#F3F4F6', borderRadius:99}}>
                <div style={{width:`${r.n}%`, height:'100%', background:'#0D9488', borderRadius:99}}/>
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
    <div style={{display:'grid', gridTemplateColumns:'repeat(3, 1fr)', gap:16}}>
      <Stat label="Completion rate" value="86%" hint="+3pp vs prev" trend="up"/>
      <Stat label="Pass rate" value="92%" hint="across 142 assessments"/>
      <Stat label="Certifications awarded" value="48" hint="last 90 days" trend="up"/>
    </div>
  </>
);

Object.assign(window, { CoordinatorDashboard, Users, SessionsMgmt, PaymentsMgmt, Reports });
