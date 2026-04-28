import { brand, font } from "../lib/api";

export const inputStyle = (focused: boolean): React.CSSProperties => ({
  width: "100%",
  padding: "10px 12px",
  fontSize: 14,
  border: `1px solid ${focused ? brand.blue : brand.borderGray}`,
  borderRadius: 8,
  color: brand.charcoal,
  background: brand.white,
  outline: "none",
  fontFamily: font,
  transition: "border-color 0.2s",
  boxSizing: "border-box",
});

export const labelStyle: React.CSSProperties = {
  display: "block",
  fontSize: 13,
  fontWeight: 500,
  color: brand.darkGray,
  marginBottom: 6,
  fontFamily: font,
};

export function Field({ label, children, error }: { label: string; children: React.ReactNode; error?: string }) {
  return (
    <div style={{ marginBottom: 16 }}>
      <label style={labelStyle}>{label}</label>
      {children}
      {error && <p style={{ margin: "4px 0 0", fontSize: 12, color: brand.red, fontFamily: font }}>{error}</p>}
    </div>
  );
}
