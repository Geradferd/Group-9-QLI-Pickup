import { brand, font, type TransportationType, type RiderResponse } from "../lib/api";
import { Field, inputStyle } from "./NewTripFormShared";

interface FormState {
  riderId: string;
  transportationTypeId: string;
  pickupAddress: string;
  destinationAddress: string;
  scheduledPickupTime: string;
  passengerCount: string;
  requiresWheelchair: boolean;
  notes: string;
}

interface FormSetters {
  setRiderId: (v: string) => void;
  setTransportationTypeId: (v: string) => void;
  setPickupAddress: (v: string) => void;
  setDestinationAddress: (v: string) => void;
  setScheduledPickupTime: (v: string) => void;
  setPassengerCount: (v: string) => void;
  setRequiresWheelchair: (v: boolean) => void;
  setNotes: (v: string) => void;
}

interface Props {
  state: FormState;
  setters: FormSetters;
  errors: Record<string, string>;
  apiError: string;
  submitting: boolean;
  isAdmin: boolean;
  riders: RiderResponse[];
  transportationTypes: TransportationType[];
  focused: Record<string, boolean>;
  focusProps: (name: string) => { onFocus: () => void; onBlur: () => void };
  onClose: () => void;
}

export default function NewTripFormBody({ state, setters, errors, apiError, submitting, isAdmin, riders, transportationTypes, focused, focusProps, onClose }: Props) {
  return (
    <>
      {apiError && (
        <div style={{ background: brand.redLight, border: `1px solid ${brand.red}`, color: brand.red, padding: "10px 14px", borderRadius: 8, fontSize: 13, marginBottom: 20, fontFamily: font }}>
          {apiError}
        </div>
      )}

      {isAdmin && (
        <Field label="Rider (optional)" error={errors.riderId}>
          <select value={state.riderId} onChange={e => setters.setRiderId(e.target.value)} style={{ ...inputStyle(!!focused.riderId), appearance: "auto" }} {...focusProps("riderId")}>
            <option value="">— Select a rider —</option>
            {riders.map(r => (
              <option key={r.id} value={r.id}>{r.firstName} {r.lastName}{r.roomNumber ? ` (Room ${r.roomNumber})` : ""}</option>
            ))}
          </select>
        </Field>
      )}

      <Field label="Transportation Type *" error={errors.transportationTypeId}>
        <select value={state.transportationTypeId} onChange={e => setters.setTransportationTypeId(e.target.value)} style={{ ...inputStyle(!!focused.transportType), appearance: "auto" }} {...focusProps("transportType")}>
          <option value="">— Select a type —</option>
          {transportationTypes.map(t => <option key={t.id} value={t.id}>{t.label}</option>)}
        </select>
      </Field>

      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
        <Field label="Pickup Address *" error={errors.pickupAddress}>
          <input type="text" value={state.pickupAddress} onChange={e => setters.setPickupAddress(e.target.value)} placeholder="123 Main St" style={inputStyle(!!focused.pickup)} {...focusProps("pickup")} />
        </Field>
        <Field label="Destination Address *" error={errors.destinationAddress}>
          <input type="text" value={state.destinationAddress} onChange={e => setters.setDestinationAddress(e.target.value)} placeholder="456 Elm Ave" style={inputStyle(!!focused.dest)} {...focusProps("dest")} />
        </Field>
      </div>

      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
        <Field label="Scheduled Pickup Time *" error={errors.scheduledPickupTime}>
          <input type="datetime-local" value={state.scheduledPickupTime} onChange={e => setters.setScheduledPickupTime(e.target.value)} style={inputStyle(!!focused.time)} {...focusProps("time")} />
        </Field>
        <Field label="Passengers *" error={errors.passengerCount}>
          <input type="number" min={1} max={20} value={state.passengerCount} onChange={e => setters.setPassengerCount(e.target.value)} style={inputStyle(!!focused.pax)} {...focusProps("pax")} />
        </Field>
      </div>

      <div style={{ marginBottom: 16, display: "flex", alignItems: "center", gap: 10 }}>
        <input id="wheelchair" type="checkbox" checked={state.requiresWheelchair} onChange={e => setters.setRequiresWheelchair(e.target.checked)} style={{ width: 16, height: 16, cursor: "pointer", accentColor: brand.blue }} />
        <label htmlFor="wheelchair" style={{ display: "block", fontSize: 13, fontWeight: 500, color: brand.darkGray, margin: 0, cursor: "pointer", fontFamily: font }}>
          ♿ Requires wheelchair accessible vehicle
        </label>
      </div>

      <Field label="Notes (optional)">
        <textarea value={state.notes} onChange={e => setters.setNotes(e.target.value)} placeholder="Any additional details…" rows={3}
          style={{ ...inputStyle(!!focused.notes), resize: "vertical", minHeight: 72 }} {...focusProps("notes")} />
      </Field>

      <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 8 }}>
        <button type="button" onClick={onClose} style={{ padding: "10px 20px", fontSize: 14, fontWeight: 500, color: brand.medGray, background: brand.white, border: `1px solid ${brand.borderGray}`, borderRadius: 8, cursor: "pointer", fontFamily: font }}>
          Cancel
        </button>
        <button type="submit" disabled={submitting} style={{ padding: "10px 24px", fontSize: 14, fontWeight: 600, color: brand.white, background: submitting ? brand.blueDark : brand.blue, border: "none", borderRadius: 8, cursor: submitting ? "default" : "pointer", fontFamily: font, opacity: submitting ? 0.8 : 1 }}>
          {submitting ? "Submitting…" : "Submit Trip Request"}
        </button>
      </div>
    </>
  );
}
