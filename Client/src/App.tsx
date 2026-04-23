import { useState, useRef, useEffect } from "react";
import DashboardPage, { type Page } from "./pages/Dashboard";
import { apiCall, brand, font, type User } from "./lib/api";

// Loading spinner component
function Spinner() {
  return (
    <svg
      width="20"
      height="20"
      viewBox="0 0 20 20"
      style={{
        animation: "spin 0.8s linear infinite",
        marginRight: 8,
        verticalAlign: "middle",
      }}
    >
      <style>{`@keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }`}</style>
      <circle cx="10" cy="10" r="8" stroke="rgba(255,255,255,0.3)" strokeWidth="2.5" fill="none" />
      <path d="M10 2 A8 8 0 0 1 18 10" stroke="#ffffff" strokeWidth="2.5" fill="none" strokeLinecap="round" />
    </svg>
  );
}

// Eye icon for show/hide password
function EyeIcon({ open }: { open: boolean }) {
  if (open) {
    return (
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke={brand.medGray} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
        <circle cx="12" cy="12" r="3" />
      </svg>
    );
  }
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke={brand.medGray} strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94" />
      <path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19" />
      <line x1="1" y1="1" x2="23" y2="23" />
    </svg>
  );
}

function LoginPage({ onLogin }: { onLogin: (user: User) => void }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const emailRef = useRef<HTMLInputElement>(null);

  // focus email field on page load
  useEffect(() => {
    emailRef.current?.focus();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email || !password) {
      setError("Please enter your email and password.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      const data = await apiCall("/auth/login", "POST", { email, password });
      onLogin(data);
    } catch {
      setError("Invalid email or password.");
    }
    setLoading(false);
  };

  const inputStyle = {
    width: "100%",
    padding: "14px 16px",
    fontSize: 16,
    border: `1px solid ${brand.borderGray}`,
    borderRadius: 6,
    color: brand.charcoal,
    background: brand.white,
    outline: "none",
    boxSizing: "border-box" as const,
    fontFamily: font,
    transition: "border-color 0.2s",
  };

  return (
    <div style={{
      minHeight: "100vh",
      display: "flex",
      flexDirection: "column" as const,
      background: brand.white,
    }}>
      {}
      <div style={{
        height: 5,
        background: `linear-gradient(90deg, ${brand.blue} 0%, ${brand.green} 40%, ${brand.gold} 100%)`,
      }} />

      <div style={{
        flex: 1,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: "48px 24px",
      }}>
        <div style={{ width: 480, maxWidth: "100%" }}>
          {}
          <div style={{ marginBottom: 48, textAlign: "center" as const }}>
            <img src="/qli-checkmark.png" alt="QLI" style={{ width: 72, height: 72, marginBottom: 12 }} />
            <h2 style={{
              fontSize: 32,
              fontWeight: 700,
              color: brand.charcoal,
              margin: "0 0 4px",
              letterSpacing: "-0.5px",
              fontFamily: font,
            }}>QLIFT</h2>
            <p style={{
              fontSize: 16,
              color: brand.medGray,
              margin: 0,
              fontFamily: font,
            }}>Transportation Coordination System</p>
          </div>

          {}
          <div style={{
            height: 1,
            background: brand.borderGray,
            marginBottom: 36,
          }} />

          {}
          <h1 style={{
            fontSize: 26,
            fontWeight: 600,
            color: brand.charcoal,
            margin: "0 0 8px",
            fontFamily: font,
          }}>Sign in to your account</h1>
          <p style={{
            fontSize: 16,
            color: brand.medGray,
            margin: "0 0 32px",
            fontFamily: font,
          }}>Enter your credentials to continue.</p>

          {}
          {error && (
            <div style={{
              background: brand.redLight,
              border: `1px solid ${brand.red}`,
              color: brand.red,
              padding: "12px 16px",
              borderRadius: 6,
              fontSize: 15,
              marginBottom: 24,
              fontFamily: font,
            }}>{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            {/* Email */}
            <div style={{ marginBottom: 22 }}>
              <label style={{
                display: "block",
                fontSize: 15,
                fontWeight: 500,
                color: brand.darkGray,
                marginBottom: 8,
                fontFamily: font,
              }}>Email address</label>
              <input
                ref={emailRef}
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@qli.org"
                autoComplete="email"
                style={inputStyle}
                onFocus={(e) => e.target.style.borderColor = brand.blue}
                onBlur={(e) => e.target.style.borderColor = brand.borderGray}
              />
            </div>

            {/* Password */}
            <div style={{ marginBottom: 32 }}>
              <div style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: 8,
              }}>
                <label style={{
                  fontSize: 15,
                  fontWeight: 500,
                  color: brand.darkGray,
                  fontFamily: font,
                }}>Password</label>
                <a href="#" onClick={(e) => e.preventDefault()} style={{
                  fontSize: 14,
                  color: brand.blue,
                  textDecoration: "none",
                  fontFamily: font,
                }}>Forgot password?</a>
              </div>
              <div style={{ position: "relative" as const }}>
                <input
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Enter your password"
                  autoComplete="current-password"
                  style={{
                    ...inputStyle,
                    paddingRight: 48,
                  }}
                  onFocus={(e) => e.target.style.borderColor = brand.blue}
                  onBlur={(e) => e.target.style.borderColor = brand.borderGray}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  style={{
                    position: "absolute" as const,
                    right: 12,
                    top: "50%",
                    transform: "translateY(-50%)",
                    background: "none",
                    border: "none",
                    cursor: "pointer",
                    padding: 4,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                  }}
                  tabIndex={-1}
                >
                  <EyeIcon open={showPassword} />
                </button>
              </div>
            </div>

            {/* Button with spinner */}
            <button
              type="submit"
              disabled={loading}
              style={{
                width: "100%",
                padding: "14px 24px",
                fontSize: 16,
                fontWeight: 600,
                color: brand.white,
                background: brand.blue,
                border: "none",
                borderRadius: 6,
                cursor: loading ? "default" : "pointer",
                fontFamily: font,
                transition: "background 0.2s",
                letterSpacing: "0.2px",
                opacity: loading ? 0.85 : 1,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
              onMouseEnter={(e) => { if (!loading) (e.currentTarget).style.background = brand.blueDark; }}
              onMouseLeave={(e) => { if (!loading) (e.currentTarget).style.background = brand.blue; }}
            >
              {loading && <Spinner />}
              {loading ? "Signing in..." : "Sign in"}
            </button>
          </form>

          <p style={{
            fontSize: 13,
            color: brand.medGray,
            textAlign: "center" as const,
            marginTop: 24,
            fontFamily: font,
          }}>Brain and Spinal Cord Injury Specialists</p>
        </div>
      </div>

      {}
      <div style={{
        padding: "20px 40px",
        borderTop: `1px solid ${brand.borderGray}`,
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}>
        <span style={{ fontSize: 13, color: brand.medGray, fontFamily: font }}>
          © 2026 Quality Living, Inc.
        </span>
        <span style={{ fontSize: 13, color: brand.medGray, fontFamily: font }}>
          QLIFT v1.0
        </span>
      </div>
    </div>
  );
}

export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const [_page, setPage] = useState<Page>("dashboard");

  if (!user) return <LoginPage onLogin={(u) => { setUser(u); setPage("dashboard"); }} />;
  return (
    <DashboardPage
      user={user}
      onLogout={() => { setUser(null); setPage("dashboard"); }}
      onNavigate={setPage}
    />
  );
}
