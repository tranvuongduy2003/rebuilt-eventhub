import type { RouteObject } from 'react-router-dom'

import { paths } from '@/app/paths'
import { TicketsPlaceholderPage } from '@/features/tickets/pages/tickets-placeholder-page'

export const ticketsRoutes: RouteObject[] = [
  { path: paths.tickets, element: <TicketsPlaceholderPage /> },
]
