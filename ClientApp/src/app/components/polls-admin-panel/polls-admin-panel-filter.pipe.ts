import {PipeTransform, Pipe } from '@angular/core'

@Pipe({
    name: 'pollFilter'
})

export class PollsAdminPanelPipe implements PipeTransform{
    
    transform(polls: Poll[], searchTerm: string): Poll[] {

        if(!polls || !searchTerm){
          return polls;
        }
    
        return polls.filter(poll =>
          poll.title.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1);
    
      
      }



}