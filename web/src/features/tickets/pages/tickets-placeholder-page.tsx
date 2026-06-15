import { Ticket } from 'lucide-react'

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export function TicketsPlaceholderPage() {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col gap-2">
        <p className="text-primary text-sm font-medium">Order fulfillment</p>
        <h1 className="text-2xl font-bold tracking-tight">Tickets</h1>
        <p className="text-muted-foreground max-w-2xl text-sm">
          Deliver digital tickets to buyers after a successful purchase.
        </p>
      </div>

      <Card className="shadow-sm">
        <CardHeader>
          <div className="bg-primary/10 text-primary mb-2 flex size-10 items-center justify-center rounded-lg">
            <Ticket className="size-5" aria-hidden />
          </div>
          <CardTitle>Coming soon</CardTitle>
          <CardDescription>
            Ticket delivery and QR display (EP-7) will live here — order reference links and mobile
            ticket view.
          </CardDescription>
        </CardHeader>
        <CardContent className="text-muted-foreground text-sm">
          Attendees will receive scannable tickets optimized for mobile, like a digital receipt.
        </CardContent>
      </Card>
    </div>
  )
}
