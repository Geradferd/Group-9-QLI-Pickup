import { useState, useRef, useEffect } from "react";

const API = "http://localhost:5270/api";

const brand = {
  blue: "#2e94b9",
  blueDark: "#267d9e",
  green: "#7ab541",
  gold: "#e8b630",
  charcoal: "#2d2d2d",
  darkGray: "#3a3a3a",
  medGray: "#6b6b6b",
  lightGray: "#f5f5f5",
  borderGray: "#dcdcdc",
  white: "#ffffff",
  error: "#c0392b",
  errorBg: "#fdf0ef",
};

async function apiCall(path: string, method = "GET", body: unknown = null, token: string | null = null) {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) headers["Authorization"] = `Bearer ${token}`;
  const opts: RequestInit = { method, headers };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(`${API}${path}`, opts);
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || `Error ${res.status}`);
  }
  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

interface User {
  id: number;
  email: string;
  displayName: string;
  role: string;
  token: string;
}

const font = "'Segoe UI', system-ui, -apple-system, sans-serif";

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
              background: brand.errorBg,
              border: `1px solid ${brand.error}`,
              color: brand.error,
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

function Dashboard({ user, onLogout }: { user: User; onLogout: () => void }) {
  return (
    <div style={{
      minHeight: "100vh",
      background: brand.lightGray,
      fontFamily: font,
    }}>
      <div style={{
        height: 5,
        background: `linear-gradient(90deg, ${brand.blue} 0%, ${brand.green} 40%, ${brand.gold} 100%)`,
      }} />

      {/* Header */}
      <div style={{
        background: brand.white,
        borderBottom: `1px solid ${brand.borderGray}`,
        padding: "16px 36px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
          <img src="/qli-checkmark.png" alt="QLI" style={{ width: 36, height: 36 }} />
          <span style={{
            fontSize: 22,
            fontWeight: 700,
            color: brand.charcoal,
            letterSpacing: "-0.3px",
          }}>QLIFT</span>
        </div>
        <div style={{ display: "flex", alignItems: "center", gap: 20 }}>
          <div style={{ textAlign: "right" as const }}>
            <div style={{ fontSize: 15, fontWeight: 500, color: brand.charcoal }}>{user.displayName}</div>
            <div style={{
              fontSize: 12,
              color: brand.blue,
              fontWeight: 600,
              textTransform: "uppercase" as const,
              letterSpacing: "0.5px",
            }}>{user.role}</div>
          </div>
          <button
            onClick={onLogout}
            style={{
              padding: "8px 18px",
              fontSize: 14,
              fontWeight: 500,
              color: brand.medGray,
              background: "transparent",
              border: `1px solid ${brand.borderGray}`,
              borderRadius: 6,
              cursor: "pointer",
              fontFamily: font,
              transition: "border-color 0.2s",
            }}
            onMouseEnter={(e) => e.currentTarget.style.borderColor = brand.medGray}
            onMouseLeave={(e) => e.currentTarget.style.borderColor = brand.borderGray}
          >Sign out</button>
        </div>
      </div>

      {/* Content */}
      <div style={{ padding: "48px 36px", maxWidth: 1000, margin: "0 auto" }}>
        <h1 style={{
          fontSize: 28,
          fontWeight: 600,
          color: brand.charcoal,
          margin: "0 0 8px",
        }}>Welcome, {user.displayName}</h1>
        <p style={{
          fontSize: 16,
          color: brand.medGray,
          margin: "0 0 36px",
        }}>You are signed in as {user.role}. Select an option below to get started.</p>

        <div style={{
          display: "grid",
          gridTemplateColumns: "1fr 1fr 1fr",
          gap: 20,
        }}>
          {[
            { title: "Riders", desc: "View and manage rider profiles", color: brand.blue },
            { title: "Drivers", desc: "View and manage driver profiles", color: brand.green },
            { title: "Vehicles", desc: "View and manage fleet vehicles", color: brand.gold },
            { title: "Trips", desc: "View and manage trip requests", color: brand.blue },
            { title: "Schedule", desc: "View daily trip schedule", color: brand.green },
            { title: "Reports", desc: "View trip reports and data", color: brand.gold },
          ].map((item) => (
            <div
              key={item.title}
              style={{
                background: brand.white,
                border: `1px solid ${brand.borderGray}`,
                borderRadius: 8,
                padding: "28px 24px",
                cursor: "pointer",
                transition: "border-color 0.2s, box-shadow 0.2s",
                borderTop: `4px solid ${item.color}`,
              }}
              onMouseEnter={(e) => {
                e.currentTarget.style.borderColor = item.color;
                e.currentTarget.style.boxShadow = "0 4px 12px rgba(0,0,0,0.07)";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.borderColor = brand.borderGray;
                e.currentTarget.style.boxShadow = "none";
              }}
            >
              <h3 style={{
                fontSize: 18,
                fontWeight: 600,
                color: brand.charcoal,
                margin: "0 0 8px",
              }}>{item.title}</h3>
              <p style={{
                fontSize: 14,
                color: brand.medGray,
                margin: 0,
                lineHeight: 1.5,
              }}>{item.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default function App() {
  const [user, setUser] = useState<User | null>(null);

  if (!user) return <LoginPage onLogin={setUser} />;
  return <Dashboard user={user} onLogout={() => setUser(null)} />;
}
