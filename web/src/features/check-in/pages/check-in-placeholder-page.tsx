import { ScanLine } from 'lucide-react'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export function CheckInPlaceholderPage() {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <p className="text-primary text-sm font-medium">Door operations</p>
        <h1 className="text-2xl font-bold tracking-tight">Check-in</h1>
        <p className="text-muted-foreground max-w-2xl text-sm">
          Validate tickets at the venue and track real-time attendance.
        </p>
      </div>

      <Card className="shadow-sm">
        <CardHeader>
          <div className="bg-primary/10 text-primary mb-2 flex size-10 items-center justify-center rounded-lg">
            <ScanLine className="size-5" aria-hidden />
          </div>
          <CardTitle>Coming soon</CardTitle>
          <CardDescription>
            Door scanning and manual lookup (EP-8) will live here — validate tickets and show door
            counts.
          </CardDescription>
        </CardHeader>
        <CardContent className="text-muted-foreground text-sm">
          Staff will scan QR codes or search by name to admit guests quickly.
        </CardContent>
      </Card>
    </div>
  )
}
