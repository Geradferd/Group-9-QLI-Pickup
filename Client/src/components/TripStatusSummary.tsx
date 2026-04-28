import { brand, font, statusColor, type TripResponse } from "../lib/api";

export function TripStatusSummary({ trips, loading }: { trips: TripResponse[]; loading: boolean }) {
  return (
    <div style={{ background: brand.white, border: `1px solid ${brand.borderGray}`, borderRadius: 12, padding: "20px", boxShadow: "0 1px 4px rgba(0,0,0,0.05)" }}>
      <h3 style={{ margin: "0 0 16px", fontSize: 15, fontWeight: 600, color: brand.charcoal, fontFamily: font }}>
        Trip Status Summary
      </h3>
      {loading ? (
        <div style={{ color: brand.medGray, fontSize: 13, fontFamily: font }}>Loading…</div>
      ) : (() => {
        const groups: Record<string, number> = {};
        trips.forEach(t => { const s = t.status || "Unknown"; groups[s] = (groups[s] || 0) + 1; });
        const total = trips.length;
        return (
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {Object.entries(groups).map(([status, count]) => {
              const pct = total > 0 ? Math.round((count / total) * 100) : 0;
              const c = statusColor(status);
              return (
                <div key={status}>
                  <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 4, fontSize: 13, fontFamily: font }}>
                    <span style={{ color: brand.darkGray, fontWeight: 500 }}>{status}</span>
                    <span style={{ color: brand.medGray }}>{count} ({pct}%)</span>
                  </div>
                  <div style={{ height: 6, background: brand.lightGray, borderRadius: 99, overflow: "hidden" }}>
                    <div style={{ height: "100%", width: `${pct}%`, background: c.dot, borderRadius: 99, transition: "width 0.6s ease" }} />
                  </div>
                </div>
              );
            })}
            {total === 0 && <div style={{ color: brand.medGray, fontSize: 13, fontFamily: font }}>No trips found</div>}
          </div>
        );
      })()}
    </div>
  );
}
