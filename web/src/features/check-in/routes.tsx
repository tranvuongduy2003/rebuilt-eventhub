import type { RouteObject } from 'react-router-dom'

import { paths } from '@/app/paths'
import { CheckInPlaceholderPage } from '@/features/check-in/pages/check-in-placeholder-page'

export const checkInRoutes: RouteObject[] = [
  { path: paths.checkIn, element: <CheckInPlaceholderPage /> },
]
