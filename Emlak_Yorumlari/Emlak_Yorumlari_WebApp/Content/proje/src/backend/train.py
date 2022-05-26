from celery import Celery
from backend.Classes.Factory import Factory

REDIS_URI = 'redis://%s:%d/%d' % ("localhost", 6379, 0)


celery = Celery(
    "worker",
    backend=REDIS_URI,
    broker=REDIS_URI,
)

@celery.task(bind=True)
def train_task(self, type, maxlen=100, batch_size=64, epoch=15):
    Factory(type, maxlen, batch_size, epoch)
    return True