import { brand, font, statusColor, statusLabel } from "../lib/api";

export function StatusBadge({ status }: { status: string }) {
  const c = statusColor(status);
  return (
    <span style={{
      display: "inline-flex",
      alignItems: "center",
      gap: 5,
      padding: "3px 10px",
      borderRadius: 99,
      fontSize: 12,
      fontWeight: 600,
      background: c.bg,
      color: c.text,
      fontFamily: font,
    }}>
      <span style={{ width: 7, height: 7, borderRadius: "50%", background: c.dot, flexShrink: 0 }} />
      {statusLabel(status)}
    </span>
  );
}

export function StatCard({
  label,
  value,
  icon,
  color,
  bg,
  sub,
}: {
  label: string;
  value: number | string;
  icon: React.ReactNode;
  color: string;
  bg: string;
  sub?: string;
}) {
  return (
    <div style={{
      background: brand.white,
      border: `1px solid ${brand.borderGray}`,
      borderRadius: 12,
      padding: "24px 20px",
      display: "flex",
      alignItems: "flex-start",
      gap: 16,
      boxShadow: "0 1px 4px rgba(0,0,0,0.05)",
    }}>
      <div style={{
        width: 48,
        height: 48,
        borderRadius: 10,
        background: bg,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        flexShrink: 0,
        color,
      }}>
        {icon}
      </div>
      <div>
        <div style={{ fontSize: 13, color: brand.medGray, fontWeight: 500, marginBottom: 4, fontFamily: font }}>{label}</div>
        <div style={{ fontSize: 28, fontWeight: 700, color: brand.charcoal, lineHeight: 1, fontFamily: font }}>{value}</div>
        {sub && <div style={{ fontSize: 12, color: brand.medGray, marginTop: 4, fontFamily: font }}>{sub}</div>}
      </div>
    </div>
  );
}

export function QuickAction({
  label,
  icon,
  color,
  bg,
  onClick,
}: {
  label: string;
  icon: React.ReactNode;
  color: string;
  bg: string;
  onClick?: () => void;
}) {
  const [hov, setHov] = React.useState(false);
  return (
    <button
      onClick={onClick}
      onMouseEnter={() => setHov(true)}
      onMouseLeave={() => setHov(false)}
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: 10,
        padding: "20px 12px",
        background: hov ? bg : brand.white,
        border: `1.5px solid ${hov ? color : brand.borderGray}`,
        borderRadius: 12,
        cursor: "pointer",
        fontFamily: font,
        transition: "all 0.15s",
        boxShadow: hov ? "0 2px 8px rgba(0,0,0,0.08)" : "none",
      }}
    >
      <div style={{
        width: 44,
        height: 44,
        borderRadius: 10,
        background: bg,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        color,
      }}>{icon}</div>
      <span style={{ fontSize: 13, fontWeight: 500, color: brand.charcoal }}>{label}</span>
    </button>
  );
}

// need React in scope for useState
import React from "react";
