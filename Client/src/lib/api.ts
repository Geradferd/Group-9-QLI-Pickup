export const API_BASE = "http://localhost:5270/api";

export const brand = {
  blue: "#2e94b9",
  blueDark: "#267d9e",
  blueLight: "#e8f4f9",
  green: "#7ab541",
  greenLight: "#f0f7e8",
  gold: "#e8b630",
  goldLight: "#fdf6e3",
  red: "#c0392b",
  redLight: "#fdf0ef",
  purple: "#8e44ad",
  purpleLight: "#f5eefb",
  charcoal: "#2d2d2d",
  darkGray: "#3a3a3a",
  medGray: "#6b6b6b",
  lightGray: "#f5f5f5",
  borderGray: "#dcdcdc",
  white: "#ffffff",
};

export const font = "'Segoe UI', system-ui, -apple-system, sans-serif";

export async function apiCall(
  path: string,
  method = "GET",
  body: unknown = null,
  token: string | null = null
) {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) headers["Authorization"] = `Bearer ${token}`;
  const opts: RequestInit = { method, headers };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(`${API_BASE}${path}`, opts);
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || `Error ${res.status}`);
  }
  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

// ── Shared Types ──────────────────────────────────────────────────────────────

export interface User {
  id: number;
  email: string;
  displayName: string;
  role: string;
  token: string;
}

export interface TripResponse {
  id: number;
  status: string;
  pickupAddress: string;
  destinationAddress: string;
  scheduledPickupTime: string;
  actualPickupTime?: string;
  actualDropoffTime?: string;
  passengerCount: number;
  requiresWheelchair: boolean;
  distanceMiles?: number;
  notes?: string;
  riderName?: string;
  driverName?: string;
}

export interface NotificationResponse {
  id: number;
  tripId?: number;
  type: string;
  title: string;
  body: string;
  isRead: boolean;
  createdAt: string;
}

export interface TransportationType {
  id: number;
  label: string;
  description?: string;
  isActive: boolean;
}

export interface RiderResponse {
  id: number;
  firstName: string;
  lastName: string;
  phone?: string;
  roomNumber?: string;
}

// ── Helpers ───────────────────────────────────────────────────────────────────

export function formatTime(dt: string) {
  return new Date(dt).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

export function formatDate(dt: string) {
  return new Date(dt).toLocaleDateString([], {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

export function timeAgo(dt: string) {
  const diff = (Date.now() - new Date(dt).getTime()) / 1000;
  if (diff < 60) return "just now";
  if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
  if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
  return `${Math.floor(diff / 86400)}d ago`;
}

export function statusColor(status: string): { bg: string; text: string; dot: string } {
  switch (status?.toLowerCase()) {
    case "pending":    return { bg: "#fff8e1",       text: "#b8860b",    dot: brand.gold };
    case "approved":   return { bg: brand.blueLight,  text: brand.blueDark, dot: brand.blue };
    case "assigned":   return { bg: "#e8f5e9",        text: "#2e7d32",    dot: "#43a047" };
    case "inprogress": return { bg: brand.purpleLight, text: brand.purple, dot: brand.purple };
    case "completed":  return { bg: brand.greenLight,  text: "#3a7d00",   dot: brand.green };
    case "cancelled":  return { bg: "#f5f5f5",         text: brand.medGray, dot: brand.medGray };
    case "denied":     return { bg: brand.redLight,    text: brand.red,    dot: brand.red };
    case "noshow":     return { bg: "#fff3f0",         text: "#c0392b",    dot: "#e74c3c" };
    default:           return { bg: brand.lightGray,   text: brand.charcoal, dot: brand.medGray };
  }
}
