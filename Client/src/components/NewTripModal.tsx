import { useState, useEffect } from "react";
import { apiCall, brand, font, type TransportationType, type RiderResponse, type User } from "../lib/api";

interface Props {
  user: User;
  onClose: () => void;
  onCreated: () => void;
}

const inputStyle = (focused: boolean): React.CSSProperties => ({
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

const labelStyle: React.CSSProperties = {
  display: "block",
  fontSize: 13,
  fontWeight: 500,
  color: brand.darkGray,
  marginBottom: 6,
  fontFamily: font,
};

function Field({
  label,
  children,
  error,
}: {
  label: string;
  children: React.ReactNode;
  error?: string;
}) {
  return (
    <div style={{ marginBottom: 16 }}>
      <label style={labelStyle}>{label}</label>
      {children}
      {error && (
        <p style={{ margin: "4px 0 0", fontSize: 12, color: brand.red, fontFamily: font }}>{error}</p>
      )}
    </div>
  );
}

export default function NewTripModal({ user, onClose, onCreated }: Props) {
  const [transportationTypes, setTransportationTypes] = useState<TransportationType[]>([]);
  const [riders, setRiders] = useState<RiderResponse[]>([]);

  const isAdmin = user.role?.toLowerCase() === "admin";

  // Form state
  const [riderId, setRiderId] = useState("");
  const [transportationTypeId, setTransportationTypeId] = useState("");
  const [pickupAddress, setPickupAddress] = useState("");
  const [destinationAddress, setDestinationAddress] = useState("");
  const [scheduledPickupTime, setScheduledPickupTime] = useState("");
  const [passengerCount, setPassengerCount] = useState("1");
  const [requiresWheelchair, setRequiresWheelchair] = useState(false);
  const [notes, setNotes] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [apiError, setApiError] = useState("");

  // Focus tracking
  const [focused, setFocused] = useState<Record<string, boolean>>({});
  const focusProps = (name: string) => ({
    onFocus: () => setFocused(p => ({ ...p, [name]: true })),
    onBlur: () => setFocused(p => ({ ...p, [name]: false })),
  });

  useEffect(() => {
    // Load transportation types and riders (admin only) in parallel
    const loads: Promise<void>[] = [
      apiCall("/transportationtypes", "GET", null, user.token)
        .then(data => setTransportationTypes((data ?? []).filter((t: TransportationType) => t.isActive)))
        .catch(() => {}),
    ];
    if (isAdmin) {
      loads.push(
        apiCall("/riders", "GET", null, user.token)
          .then(data => setRiders(data ?? []))
          .catch(() => {})
      );
    }
    Promise.all(loads);
  }, []);

  function validate() {
    const e: Record<string, string> = {};
    if (!transportationTypeId) e.transportationTypeId = "Please select a transportation type.";
    if (!pickupAddress.trim()) e.pickupAddress = "Pickup address is required.";
    if (!destinationAddress.trim()) e.destinationAddress = "Destination address is required.";
    if (!scheduledPickupTime) e.scheduledPickupTime = "Please select a pickup date and time.";
    const pax = parseInt(passengerCount);
    if (isNaN(pax) || pax < 1 || pax > 20) e.passengerCount = "Passenger count must be 1–20.";
    return e;
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length > 0) { setErrors(errs); return; }
    setErrors({});
    setApiError("");
    setSubmitting(true);
    try {
      await apiCall("/trips", "POST", {
        riderId: riderId ? parseInt(riderId) : null,
        transportationTypeId: parseInt(transportationTypeId),
        pickupAddress: pickupAddress.trim(),
        destinationAddress: destinationAddress.trim(),
        scheduledPickupTime: new Date(scheduledPickupTime).toISOString(),
        passengerCount: parseInt(passengerCount),
        requiresWheelchair,
        notes: notes.trim() || null,
      }, user.token);
      onCreated();
      onClose();
    } catch (err: unknown) {
      setApiError(err instanceof Error ? err.message : "Failed to create trip.");
    }
    setSubmitting(false);
  }

  // Default the datetime input to now + 1 hour, rounded to nearest 15 min
  useEffect(() => {
    const d = new Date();
    d.setHours(d.getHours() + 1);
    d.setMinutes(Math.ceil(d.getMinutes() / 15) * 15, 0, 0);
    const local = new Date(d.getTime() - d.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16);
    setScheduledPickupTime(local);
  }, []);

  return (
    // Backdrop
    <div
      onClick={onClose}
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(0,0,0,0.45)",
        zIndex: 1000,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 24,
      }}
    >
      {/* Modal */}
      <div
        onClick={e => e.stopPropagation()}
        style={{
          background: brand.white,
          borderRadius: 16,
          width: "100%",
          maxWidth: 560,
          maxHeight: "90vh",
          display: "flex",
          flexDirection: "column",
          boxShadow: "0 20px 60px rgba(0,0,0,0.25)",
          overflow: "hidden",
        }}
      >
        {/* Header */}
        <div style={{
          padding: "20px 24px",
          borderBottom: `1px solid ${brand.borderGray}`,
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          flexShrink: 0,
        }}>
          <div>
            <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, color: brand.charcoal, fontFamily: font }}>
              New Trip Request
            </h2>
            <p style={{ margin: "2px 0 0", fontSize: 13, color: brand.medGray, fontFamily: font }}>
              Fill in the details below to schedule a trip.
            </p>
          </div>
          <button
            onClick={onClose}
            style={{
              width: 32, height: 32, borderRadius: "50%",
              border: `1px solid ${brand.borderGray}`,
              background: brand.white,
              cursor: "pointer",
              display: "flex", alignItems: "center", justifyContent: "center",
              fontSize: 18, color: brand.medGray, lineHeight: 1,
            }}
          >×</button>
        </div>

        {/* Body */}
        <form onSubmit={handleSubmit} style={{ overflowY: "auto", flex: 1, padding: "24px" }}>

          {apiError && (
            <div style={{
              background: brand.redLight,
              border: `1px solid ${brand.red}`,
              color: brand.red,
              padding: "10px 14px",
              borderRadius: 8,
              fontSize: 13,
              marginBottom: 20,
              fontFamily: font,
            }}>{apiError}</div>
          )}

          {/* Rider (admin only) */}
          {isAdmin && (
            <Field label="Rider (optional)" error={errors.riderId}>
              <select
                value={riderId}
                onChange={e => setRiderId(e.target.value)}
                style={{ ...inputStyle(!!focused.riderId), appearance: "auto" }}
                {...focusProps("riderId")}
              >
                <option value="">— Select a rider —</option>
                {riders.map(r => (
                  <option key={r.id} value={r.id}>
                    {r.firstName} {r.lastName}{r.roomNumber ? ` (Room ${r.roomNumber})` : ""}
                  </option>
                ))}
              </select>
            </Field>
          )}

          {/* Transportation type */}
          <Field label="Transportation Type *" error={errors.transportationTypeId}>
            <select
              value={transportationTypeId}
              onChange={e => setTransportationTypeId(e.target.value)}
              style={{ ...inputStyle(!!focused.transportType), appearance: "auto" }}
              {...focusProps("transportType")}
            >
              <option value="">— Select a type —</option>
              {transportationTypes.map(t => (
                <option key={t.id} value={t.id}>{t.label}</option>
              ))}
            </select>
          </Field>

          {/* Pickup & Destination */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <Field label="Pickup Address *" error={errors.pickupAddress}>
              <input
                type="text"
                value={pickupAddress}
                onChange={e => setPickupAddress(e.target.value)}
                placeholder="123 Main St"
                style={inputStyle(!!focused.pickup)}
                {...focusProps("pickup")}
              />
            </Field>
            <Field label="Destination Address *" error={errors.destinationAddress}>
              <input
                type="text"
                value={destinationAddress}
                onChange={e => setDestinationAddress(e.target.value)}
                placeholder="456 Elm Ave"
                style={inputStyle(!!focused.dest)}
                {...focusProps("dest")}
              />
            </Field>
          </div>

          {/* Date/Time & Passengers */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <Field label="Scheduled Pickup Time *" error={errors.scheduledPickupTime}>
              <input
                type="datetime-local"
                value={scheduledPickupTime}
                onChange={e => setScheduledPickupTime(e.target.value)}
                style={inputStyle(!!focused.time)}
                {...focusProps("time")}
              />
            </Field>
            <Field label="Passengers *" error={errors.passengerCount}>
              <input
                type="number"
                min={1}
                max={20}
                value={passengerCount}
                onChange={e => setPassengerCount(e.target.value)}
                style={inputStyle(!!focused.pax)}
                {...focusProps("pax")}
              />
            </Field>
          </div>

          {/* Wheelchair */}
          <div style={{ marginBottom: 16, display: "flex", alignItems: "center", gap: 10 }}>
            <input
              id="wheelchair"
              type="checkbox"
              checked={requiresWheelchair}
              onChange={e => setRequiresWheelchair(e.target.checked)}
              style={{ width: 16, height: 16, cursor: "pointer", accentColor: brand.blue }}
            />
            <label htmlFor="wheelchair" style={{ ...labelStyle, margin: 0, cursor: "pointer" }}>
              ♿ Requires wheelchair accessible vehicle
            </label>
          </div>

          {/* Notes */}
          <Field label="Notes (optional)">
            <textarea
              value={notes}
              onChange={e => setNotes(e.target.value)}
              placeholder="Any additional details…"
              rows={3}
              style={{
                ...inputStyle(!!focused.notes),
                resize: "vertical",
                minHeight: 72,
              }}
              {...focusProps("notes")}
            />
          </Field>

          {/* Footer buttons */}
          <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 8 }}>
            <button
              type="button"
              onClick={onClose}
              style={{
                padding: "10px 20px",
                fontSize: 14,
                fontWeight: 500,
                color: brand.medGray,
                background: brand.white,
                border: `1px solid ${brand.borderGray}`,
                borderRadius: 8,
                cursor: "pointer",
                fontFamily: font,
              }}
            >Cancel</button>
            <button
              type="submit"
              disabled={submitting}
              style={{
                padding: "10px 24px",
                fontSize: 14,
                fontWeight: 600,
                color: brand.white,
                background: submitting ? brand.blueDark : brand.blue,
                border: "none",
                borderRadius: 8,
                cursor: submitting ? "default" : "pointer",
                fontFamily: font,
                opacity: submitting ? 0.8 : 1,
                transition: "background 0.2s",
              }}
            >
              {submitting ? "Submitting…" : "Submit Trip Request"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
