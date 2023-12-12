import React, { useState } from 'react'
import './Notification.css'

type NotificationType = 'success' | 'error'

interface NotificationProps {
    message: string,
    type: NotificationType
  }

const Notification = (props: NotificationProps) => {
    const { message, type } = props
    if (message === '')
        return <></>

    const color = type === 'error' 
        ? '#DB504A ' 
        : '#9BC53D'
        

    return (
        <div className='notification' style={{backgroundColor: color}}>{message}</div>
    )
}
const defaultNotification: NotificationProps = { type: "error", message: '' }
export const useNotification = () => {
    const [notification, setNotification] = useState(defaultNotification);
    const displayNotification = (msg: string, type: NotificationType) => {
        setNotification({ message: msg, type: type })
        setTimeout(() => setNotification(defaultNotification), 5_000)
}

return {
    notify: displayNotification, 
    notification: notification
}
}

export default Notification