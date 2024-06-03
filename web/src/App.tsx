
import { ThemeProvider } from '@lobehub/ui'
import './App.css'
import { RouterProvider, createBrowserRouter } from 'react-router-dom'
import MainLayout from './_layout'
import Nav from './components/@nav/default'
import Home from './pages/home/page'
import SearchPage from './pages/search/page'
import AuthPage from './pages/auth/page'
import CurrentPage from './pages/current/page'

const router = createBrowserRouter([{
  element: <MainLayout nav={<Nav />} />,
  children: [
    {
      path: '/',
      element: <Home />
    },
    {
      path: '/home',
      element: <Home />
    },
    {
      path: '/search',
      element: <SearchPage />
    },
    {
      path: '/current',
      element: <CurrentPage />
    }
  ],
}, {
  path: '/auth/github',
  element: <AuthPage />
}
])

function App() {
  return (
    <ThemeProvider themeMode='auto' style={{
      height: '100%'
    }}>
      <RouterProvider router={router} />
    </ThemeProvider>
  )
}

export default App