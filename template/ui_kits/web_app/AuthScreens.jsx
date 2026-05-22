/* global React, I, Btn, Field, Input */
const { useState } = React;

const AuthSplit = ({ children }) => (
  <div style={{display:'grid', gridTemplateColumns:'1fr 1fr', minHeight:'100vh', background:'#fff'}}>
    <div style={{display:'flex', flexDirection:'column', padding:'32px 48px', justifyContent:'space-between'}}>
      <img src="../../assets/logo.svg" alt="Taalam" style={{height:28}}/>
      <div style={{maxWidth:400, width:'100%', alignSelf:'center', flex:1, display:'flex', flexDirection:'column', justifyContent:'center'}}>
        {children}
      </div>
      <div style={{font:'400 12px/16px Inter, sans-serif', color:'#9CA3AF'}}>© 2026 Taalam Training Institute · Manama, Bahrain</div>
    </div>
    <div style={{position:'relative', backgroundImage:'url(../../assets/auth-panel.svg)', backgroundSize:'cover', backgroundPosition:'center', padding:48, color:'#fff', display:'flex', flexDirection:'column', justifyContent:'flex-end'}}>
      <div style={{font:'600 11px/14px Inter, sans-serif', textTransform:'uppercase', letterSpacing:'0.08em', opacity:0.85, marginBottom:12}}>Training & certification</div>
      <div style={{font:'700 32px/40px Inter, sans-serif', letterSpacing:'-0.01em', maxWidth:380, marginBottom:14}}>
        Manage every step from enrollment to certification.
      </div>
      <div style={{font:'400 15px/24px Inter, sans-serif', opacity:0.88, maxWidth:380}}>
        Bahrain's professional training platform for trainees, instructors, and coordinators.
      </div>
    </div>
  </div>
);

const Login = ({ onLogin }) => {
  return (
    <AuthSplit>
      <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', marginBottom:8}}>Sign in to Taalam</div>
      <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginBottom:28}}>Welcome back. Enter your credentials to continue.</div>

      <div style={{display:'flex', flexDirection:'column', gap:14}}>
        <Field label="Email address"><Input defaultValue="user@taalam.bh"/></Field>
        <Field label="Password"><Input type="password" defaultValue="••••••••••"/></Field>

        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginTop:4}}>
          <label style={{display:'flex', alignItems:'center', gap:8, font:'400 13px/20px Inter, sans-serif', color:'#1A1A2E'}}>
            <input type="checkbox" style={{accentColor:'#0D9488', width:14, height:14}}/> Remember me
          </label>
          <a href="#" style={{font:'500 13px/20px Inter, sans-serif', color:'#0D9488', textDecoration:'none'}}>Forgot password?</a>
        </div>

        <Btn full onClick={()=>onLogin('Trainee')}>Sign in</Btn>
        <div style={{textAlign:'center', font:'400 13px/20px Inter, sans-serif', color:'#6B7280', marginTop:6}}>
          New to Taalam? <a href="#" onClick={(e)=>{e.preventDefault(); onLogin('register');}} style={{color:'#0D9488', fontWeight:600, textDecoration:'none'}}>Create an account</a>
        </div>
      </div>
    </AuthSplit>
  );
};

const Register = ({ onLogin, onBack }) => (
  <AuthSplit>
    <div style={{font:'700 28px/36px Inter, sans-serif', color:'#1A1A2E', letterSpacing:'-0.01em', marginBottom:8}}>Create your account</div>
    <div style={{font:'400 14px/22px Inter, sans-serif', color:'#6B7280', marginBottom:28}}>Join as a trainee. Instructors and coordinators are invited by Taalam staff.</div>
    <div style={{display:'grid', gridTemplateColumns:'1fr 1fr', gap:14}}>
      <Field label="First name"><Input defaultValue="Ahmed"/></Field>
      <Field label="Last name"><Input defaultValue="Alhalal"/></Field>
      <div style={{gridColumn:'1 / -1'}}><Field label="Email"><Input defaultValue="omar@example.bh"/></Field></div>
      <Field label="Date of birth"><Input type="text" defaultValue="14/02/1995"/></Field>
      <Field label="Phone"><Input defaultValue="+973 3300 1234"/></Field>
      <div style={{gridColumn:'1 / -1'}}><Field label="Password" hint="Minimum 8 characters with one number."><Input type="password" defaultValue="••••••••••"/></Field></div>
      <div style={{gridColumn:'1 / -1', display:'flex', alignItems:'flex-start', gap:8, font:'400 13px/20px Inter, sans-serif', color:'#1A1A2E'}}>
        <input type="checkbox" style={{accentColor:'#0D9488', width:14, height:14, marginTop:3}} defaultChecked/>
        <span>I agree to the <a href="#" style={{color:'#0D9488'}}>terms of service</a> and <a href="#" style={{color:'#0D9488'}}>privacy policy</a>.</span>
      </div>
      <div style={{gridColumn:'1 / -1', display:'flex', gap:10}}>
        <Btn variant="secondary" onClick={onBack}>Back to sign in</Btn>
        <Btn onClick={()=>onLogin('Trainee')}>Create account</Btn>
      </div>
    </div>
  </AuthSplit>
);

Object.assign(window, { Login, Register });
